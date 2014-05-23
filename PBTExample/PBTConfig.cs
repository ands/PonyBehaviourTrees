using System.Windows.Forms;
using System.Diagnostics;

namespace PBTExample
{
    internal static class PBTConfig
    {
        public static readonly PBT.ILogger Logger = new MessageBoxLogger();

        // the .NET usings for the code inside the PBTs
        public static readonly string[] Usings = new string[]
        {
            "System",
            "System.Drawing",
            "PBTExample"
        };

        private class MessageBoxLogger : PBT.ILogger
        {
            public void Error(string info, params object[] args)
            {
                MessageBox.Show(string.Format(info, args), "PBT Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //if (Debugger.IsAttached)
                //    Debugger.Break();
            }

            public void Info(string info, params object[] args)
            {
                MessageBox.Show(string.Format(info, args), "PBT Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //if (Debugger.IsAttached)
                //    Debugger.Break();
            }

            public void Warning(string info, params object[] args)
            {
                MessageBox.Show(string.Format(info, args), "PBT Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //if (Debugger.IsAttached)
                //    Debugger.Break();
            }
        }
    }
}
