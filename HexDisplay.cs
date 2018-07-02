using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace ClipboardTest
{
    public partial class HexDisplay : UserControl
    {
        private int maxLength;

        [Description("Gets or sets the maximum number of bytes to be displayed as hexadecimal values. Valid values are from 16 to (int.MaxValue / 2).")]
        [DefaultValue(16 << 10)]
        public int MaxLength
        {
            get { return maxLength; }
            set
            {
                // validate
                if (value < 16 || value > (int.MaxValue >> 1))
                {
                    throw new ArgumentOutOfRangeException("MaxLength");
                }

                this.maxLength = value;
            }
        }

        public HexDisplay()
        {
            this.maxLength = 16 << 10;

            InitializeComponent();
        }

        [Browsable(false)]
        public byte[] Data
        {
            set
            {
                if (value == null || value.Length < 1)
                {
                    this.richTextBox1.Text = string.Empty;
                }
                else
                {
                    int max = (int)Math.Min(value.LongLength, this.MaxLength);

                    // ensure line get's printed completely (i.e. if more than one line)
                    // -> align on 16-byte boundary
                    if (max > 16)
                    {
                        max &= ~(0x000F);
                    }

                    StringBuilder sb = new StringBuilder(max << 2);
                    StringBuilder tmp = new StringBuilder(16);
                    for (int i = 0, col = 0; i < max; ++i, ++col)
                    {
                        // line number
                        if (col == 0)
                        {
                            sb.Append(i.ToString("X8")).Append("   ");
                        }
                        byte val = value[i];
                        sb.Append(val.ToString("X2")).Append(" ");
                        char chr = Convert.ToChar(val);
                        if (char.IsControl(chr))
                        {
                            chr = '.';
                        }
                        tmp.Append(chr);
                        if (col == 7)
                        {
                            sb.Append(" ");
                        }
                        else if (col == 15)
                        {
                            sb.Append("  ");
                            sb.AppendLine(tmp.ToString());
                            tmp.Clear();
                            col = -1;
                        }
                    }
                    if (tmp.Length > 0)
                    {
                        // append remainder of data
                        while (tmp.Length < 8)
                        {
                            sb.Append("   ");
                            tmp.Append(" ");
                        }
                        if (tmp.Length == 8)
                        {
                            sb.Append(" ");
                        }
                        while (tmp.Length < 16)
                        {
                            sb.Append("   ");
                            tmp.Append(" ");
                        }

                        sb.Append("  ");
                        sb.AppendLine(tmp.ToString());
                    }

                    this.richTextBox1.Text = sb.ToString();
                }
            }
        }
    }
}
