using Common;
using System;
using System.Collections.Generic;
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

namespace WpfCore
{
    /// <summary>
    /// BluetoothWin.xaml 的交互逻辑
    /// </summary>
    public partial class BluetoothWin : Window
    {
        public BluetoothWin()
        {
            InitializeComponent();
        }

        private void TestBtnClick(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                BluetoothHelper bluetooth = new BluetoothHelper();
                try
                {
                    bluetooth.InTheHandBluetoothLE();
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message);
                }
            });
        }

        private void ShowMsg(string msg)
        {
            Dispatcher.Invoke(() =>
            {
                textTb.Text = msg;
            });
        }
    }
}
