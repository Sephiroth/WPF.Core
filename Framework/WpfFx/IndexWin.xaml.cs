using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfFx.Win;

namespace WpfFx
{
    /// <summary>
    /// IndexWin.xaml 的交互逻辑
    /// </summary>
    public partial class IndexWin : Window
    {
        public IndexWin()
        {
            InitializeComponent();
        }

        private void TreeViewBtnClick(object sender, RoutedEventArgs e)
        {
            TreeViewWin win = new TreeViewWin();
            win.Show();
        }

        private void GifWinBtnClick(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow();
            win.Show();
        }
    }
}
