//#define LEGACY_CLIPBOARD_VIEWER
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClipboardTest
{
    //[Serializable]
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void UpdateDisplayFormats()
        {
            if (!this.Visible)
            {
                return;
            }
            try
            {
                // remeber selected item
                string selectedItem = this.listBoxFormats.SelectedItem as string;

                IDataObject dataObject = Clipboard.GetDataObject();
                this.listBoxFormats.SelectedIndex = -1;
                this.listBoxFormats.Items.Clear();
                this.listBoxFormats.Items.AddRange(dataObject.GetFormats(!this.checkBoxNativeOnly.Checked));

                // re-select previous item, if any
                //int index = this.listBoxFormats.Items.IndexOf(selectedItem);
                //if (index > -1)
                {
                    this.listBoxFormats.SelectedItem = selectedItem;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UpdateCurrentContent()
        {
            string dataFormat = this.listBoxFormats.SelectedItem as string;
            string stringData = null;
            string rtfData = null;
            string htmlData = null;
            Image bmpData = null;
            object objectData = null;
            byte[] binData = null;
            string[] lines = null;

            // hide controls
            this.richTextBox1.Visible = false;
            this.textBox1.Visible = false;
            this.browserControl1.Visible = false;
            this.pictureBox1.Visible = false;
            this.propertyGrid1.Visible = false;
            this.hexBox.Visible = false;

            // reset control contents
            this.richTextBox1.Text = string.Empty;
            this.textBox1.Text = string.Empty;
            this.browserControl1.DocumentText = string.Empty;
            this.pictureBox1.Image = null;
            this.propertyGrid1.SelectedObject = null;
            this.hexBox.Data = null;

            try
            {
                // strings
                if (dataFormat == DataFormats.StringFormat || dataFormat == DataFormats.Text || dataFormat == DataFormats.UnicodeText || dataFormat == DataFormats.OemText)
                {
                    stringData = Clipboard.GetText();
                }
                // CSV
                else if (dataFormat == DataFormats.CommaSeparatedValue || dataFormat == "XML Spreadsheet")
                {
                    object format = Clipboard.GetData(dataFormat);
                    if (format is MemoryStream)
                    {
                        // TODO: display in gridview
                        using (StreamReader sr = new StreamReader((MemoryStream)format))
                        {
                            stringData = sr.ReadToEnd();
                        }
                    }
                    else if (format is string)
                    {
                        stringData = (string)format;
                    }
                    else
                    {
                        Debug.Fail("DataFormats.CommaSeparatedValue but does not contain a MemoryStream. Please investigate!");
                    }
                }
                // HTML
                else if (dataFormat == DataFormats.Html)
                {
                    htmlData = Clipboard.GetData(dataFormat) as string;
                }
                // RTF
                else if (dataFormat == DataFormats.Rtf)
                {
                    rtfData = Clipboard.GetData(dataFormat) as string;
                }
                // images
                else if (dataFormat == DataFormats.Bitmap || dataFormat == "System.Drawing.Bitmap" || dataFormat == DataFormats.EnhancedMetafile || dataFormat == DataFormats.Dib || dataFormat == DataFormats.MetafilePict || dataFormat == DataFormats.Tiff)
                {
                    //bmpData = Clipboard.GetData(dataFormat) as Bitmap;
                    bmpData = Clipboard.GetImage();

                    // the following doesn't work!
                    // Clipboard.GetData() throws an exception that crashes the application, even with try/catch block :(
                    //if (bmpData == null)
                    //{
                    //    try
                    //    {
                    //        object format = Clipboard.GetData(dataFormat);
                    //        if (format is MemoryStream)
                    //        {
                    //            bmpData = Image.FromStream((MemoryStream)format);
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        MessageBox.Show(ex.Message);
                    //    }
                    //}
                }
                // .NET object
                else if (dataFormat == DataFormats.Serializable)
                {
                    objectData = Clipboard.GetData(dataFormat);
                }
                else if (!string.IsNullOrEmpty(dataFormat))
                {
                    object format = Clipboard.GetData(dataFormat);
                    if (format is MemoryStream)
                    {
                        binData = ((MemoryStream)format).ToArray();
                    }
                    else if (format is string)
                    {
                        stringData = (string)format;
                    }
                    else if (format is string[])
                    {
                        lines = (string[])format;
                    }
                    else
                    {
                        objectData = format;
                    }
                }

                if (!string.IsNullOrEmpty(stringData))
                {
                    this.textBox1.Text = stringData;
                    this.textBox1.Visible = true;
                }
                else if (!string.IsNullOrEmpty(rtfData))
                {
                    this.richTextBox1.Rtf = rtfData;
                    this.richTextBox1.Visible = true;
                }
                else if (!string.IsNullOrEmpty(htmlData))
                {
                    this.browserControl1.Visible = true;
                    // wait for browser control to show up (max. 1s)
                    Stopwatch sw = Stopwatch.StartNew();
                    while (this.browserControl1.IsBusy && sw.ElapsedMilliseconds < 1000)
                    {
                        Application.DoEvents();
                    }
                    // wait for browser control to finish loading initial state, if any (max. 1s)
                    sw.Restart();
                    while (this.browserControl1.ReadyState == WebBrowserReadyState.Loading && sw.ElapsedMilliseconds < 1000)
                    {
                        Application.DoEvents();
                    }
                    sw.Stop();
                    int start = htmlData.IndexOf('<');
                    if (start > 0)
                    {
                        this.browserControl1.DocumentText = htmlData.Substring(start);
                    }
                    else
                    {
                        this.browserControl1.DocumentText = htmlData;
                    }
                }
                else if (bmpData != null)
                {
                    this.pictureBox1.Image = bmpData;
                    this.pictureBox1.Visible = true;
                }
                else if (objectData != null)
                {
                    this.propertyGrid1.SelectedObject = objectData;
                    this.propertyGrid1.Visible = true;
                }
                else if (binData != null)
                {
                    this.hexBox.Data = binData;
                    this.hexBox.Visible = true;
                }
                else if (lines != null && lines.Length > 0)
                {
                    this.textBox1.Lines = lines;
                    this.textBox1.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void linkLabelClear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.Clear();
            this.UpdateDisplayFormats();
        }

        private void listBoxFormats_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateCurrentContent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // adjust location, size and visibility
            this.richTextBox1.Width = this.listBoxFormats.Width;

            this.textBox1.Location = this.richTextBox1.Location;
            this.textBox1.Size = this.richTextBox1.Size;

            this.browserControl1.Location = this.richTextBox1.Location;
            this.browserControl1.Size = this.richTextBox1.Size;

            this.pictureBox1.Location = this.richTextBox1.Location;
            this.pictureBox1.Size = this.richTextBox1.Size;

            this.propertyGrid1.Location = this.richTextBox1.Location;
            this.propertyGrid1.Size = this.richTextBox1.Size;

            this.hexBox.Location = this.richTextBox1.Location;
            this.hexBox.Size = this.richTextBox1.Size;

            this.richTextBox1.Visible = false;
            this.textBox1.Visible = false;
            this.browserControl1.Visible = false;
            this.pictureBox1.Visible = false;
            this.propertyGrid1.Visible = false;
            this.hexBox.Visible = false;

            this.UpdateDisplayFormats();
        }

        private void checkBoxNativeOnly_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateDisplayFormats();
        }

#if (LEGACY_CLIPBOARD_VIEWER)
        [DllImport("user32.dll")]
        static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32.dll")]
        static extern IntPtr GetClipboardViewer();  // not actually used

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private IntPtr hWndNextWindow;  // next window in chain

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (0x0001): // WM_CREATE
                    hWndNextWindow = SetClipboardViewer(this.Handle);
                    break;
                case (0x0002): // WM_DESTROY
                    ChangeClipboardChain(this.Handle, hWndNextWindow);
                    break;
                case (0x030D): // WM_CHANGECBCHAIN
                    if (m.WParam == hWndNextWindow)
                    {
                        hWndNextWindow = m.LParam;
                    }
                    else if (hWndNextWindow != IntPtr.Zero)
                    {
                        SendMessage(hWndNextWindow, m.Msg, m.WParam, m.LParam);
                    }
                    break;
                case (0x0308): // WM_DRAWCLIPBOARD
                    this.UpdateDisplayFormats();
                    if (hWndNextWindow != IntPtr.Zero)
                    {
                        SendMessage(hWndNextWindow, m.Msg, m.WParam, m.LParam);
                    }
                    break;
            }
            base.WndProc(ref m);
        }
#else
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (0x0001): // WM_CREATE
                    AddClipboardFormatListener(this.Handle);
                    break;
                case (0x0002): // WM_DESTROY
                    RemoveClipboardFormatListener(this.Handle);
                    break;
                case (0x031D): // WM_CLIPBOARDUPDATE
                    this.UpdateDisplayFormats();
                    break;
            }
            base.WndProc(ref m);
        }
#endif
    }
}
