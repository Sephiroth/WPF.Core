using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
//using WinFormsCore.OpenGLWin;

namespace WinFormsCore
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MenuForm());

            //MainWin win = new MainWin(GameWindowSettings.Default, NativeWindowSettings.Default);
            //win.Run();

        }
    }
}
