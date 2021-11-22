using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfFx.Models;

namespace WpfFx.ViewModels
{
    public class TreeViewWinViewModel : BindableBase
    {
        private List<TreeNodeModel> treeNodes;
        public List<TreeNodeModel> TreeNodes
        {
            get { return treeNodes; }
            set { SetProperty(ref treeNodes, value); }
        }

    }
}