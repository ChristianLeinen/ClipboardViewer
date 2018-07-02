using System;
using System.Windows.Forms;

namespace ClipboardTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            //System.Diagnostics.Trace.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.SendTo));

            //System.Reflection.Assembly asm = System.Reflection.Assembly.GetEntryAssembly();
            //System.Reflection.AssemblyName asmName = asm.GetName();
            //byte[] key = asmName.GetPublicKey();
            //if (key != null && key.Length > 0)
            //{
            //    // signiert
            //}

            //foreach (var attribute in asm.GetCustomAttributes(false))
            //{
            //    System.Diagnostics.Trace.WriteLine(attribute);

            //    if (attribute is System.Diagnostics.DebuggableAttribute)
            //    {
            //        if (((System.Diagnostics.DebuggableAttribute)attribute).IsJITTrackingEnabled)
            //        {
            //            System.Diagnostics.Trace.WriteLine("DEBUG MODE!");
            //        }
            //    }
            //}
        }
    }
}
