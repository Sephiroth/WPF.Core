using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsCore
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void FormLoad(object sender, EventArgs e)
        {
            OpenTK.Windowing.Desktop.NativeWindow nativeWin = new OpenTK.Windowing.Desktop.NativeWindow(OpenTK.Windowing.Desktop.NativeWindowSettings.Default);
            nativeWin.CenterWindow();
        }
    }
}
