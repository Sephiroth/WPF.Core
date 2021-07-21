using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinFormsCore
{
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();
        }

        private void browserBtn_Click(object sender, EventArgs e)
        {
            MainForm form = new MainForm();
            form.ShowDialog();
            form.Dispose();
        }
    }
}
