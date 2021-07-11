using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinFormsCore
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //OpenTK.Windowing.Desktop.NativeWindow nativeWin = new OpenTK.Windowing.Desktop.NativeWindow(OpenTK.Windowing.Desktop.NativeWindowSettings.Default);
            //nativeWin.CenterWindow(new OpenTK.Mathematics.Vector2i(1280, 720));

            OpenTK.GLControl videoGrid = new OpenTK.GLControl();
            videoGrid.Width = 200;
            videoGrid.Height = 180;
            videoGrid.AutoScrollPosition = new Point(50, 80);
            videoGrid.ForeColor = Color.Red;
            videoGrid.Click += (obj, args) =>
            {
                MessageBox.Show("OpenTK.GLControl当作按钮", "提示", MessageBoxButtons.OK);
            };
            
            //videoGrid.ForeColor

            //this.Container.Add(videoGrid);
            this.Controls.Add(videoGrid);


            

        }
    }
}
