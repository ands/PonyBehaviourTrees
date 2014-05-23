using System;
using System.Windows.Forms;

namespace PBTExample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnGuiUnhandedException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void OnUnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
                MessageBox.Show(ex.ToString(), "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void OnGuiUnhandedException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if(e.Exception != null)
                MessageBox.Show(e.Exception.ToString(), "Unhandled GUI Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
