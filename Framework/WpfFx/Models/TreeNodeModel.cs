using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFx.Models
{
    public class TreeNodeModel : BindableBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value); }
        }

        public List<TreeNodeModel> SubNodes { get; set; } = new List<TreeNodeModel>();

    }
}