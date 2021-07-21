using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfCore
{
    /// <summary>
    /// MenuWin.xaml 的交互逻辑
    /// </summary>
    public partial class MenuWin : Window
    {
        public MenuWin()
        {
            InitializeComponent();
        }

        private void OpenCvBtnClick(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow();
            win.ShowDialog();
        }

        private void OpenALBtnClick(object sender, RoutedEventArgs e)
        {
            OpenALWin win = new OpenALWin();
            win.ShowDialog();
        }
        
    }
}