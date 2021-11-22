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
using WpfFx.ViewModels;

namespace WpfFx.Win
{
    /// <summary>
    /// TreeViewWin.xaml 的交互逻辑
    /// </summary>
    public partial class TreeViewWin : Window
    {
        private readonly TreeViewWinViewModel vm;
        public TreeViewWin()
        {
            InitializeComponent();
            vm = new TreeViewWinViewModel();
            DataContext = vm;
        }

        private void WinLoaded(object sender, RoutedEventArgs e)
        {
            List<Models.TreeNodeModel> nodes = new List<Models.TreeNodeModel>();
            nodes.Add(new Models.TreeNodeModel
            {
                Id = 1,
                Name = "root",
            });
            nodes[0].SubNodes.Add(new Models.TreeNodeModel { Id = 2, Name = "唐诗" });
            nodes[0].SubNodes[0].SubNodes.Add(new Models.TreeNodeModel { Id = 3, Name = "李白" });
            nodes[0].SubNodes[0].SubNodes[0].SubNodes.Add(new Models.TreeNodeModel { Id = 4, Name = "侠客行" });
            vm.TreeNodes = nodes;
        }

    }
}
