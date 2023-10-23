using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextSearcher
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var app = new Svcs.Lanucher();
            app.Init();
            Application.Run(new Views.FormMain());
            app.Dispose();
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SetProcessDPIAware();
    }
}
