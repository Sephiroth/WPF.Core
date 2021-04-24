using Prism.Commands;
using Prism.Mvvm;

namespace ViewModel.Core.Win
{
    public class MainWinViewModel : BindableBase
    {
        public MainWinViewModel()
        {
            winTitle = "初始测试窗口";
            ShowHello = new DelegateCommand(ShowHelloCmd);
        }

        #region bind属性
        private string winTitle;
        public string WinTitle
        {
            get { return winTitle; }
            set { SetProperty(ref winTitle, value); }
        }
        #endregion

        #region
        public DelegateCommand ShowHello { get; set; }
        private void ShowHelloCmd()
        {
            WinTitle = "来自MainWinViewModel->showHello获取的标题";
        }
        #endregion
    }
}