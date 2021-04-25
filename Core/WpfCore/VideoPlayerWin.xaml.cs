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
    /// VideoPlayerWin.xaml 的交互逻辑
    /// </summary>
    public partial class VideoPlayerWin : Window
    {
        public VideoPlayerWin()
        {
            InitializeComponent();

            mediaPlayer.LoadedBehavior = MediaState.Manual;
            mediaPlayer.UnloadedBehavior = MediaState.Manual;
        }

        private void OpenFileBtnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "视频文件|*.mp4;*.mp3;*.avi;*.mov;*.rmvb;*.flv"
            };
            bool? rs = dialog.ShowDialog();
            if (rs.HasValue && rs.Value)
            {
                selectedFileTb.Text = dialog.FileName;
                mediaPlayer.Source = new Uri(selectedFileTb.Text, UriKind.Relative);
            }
            else
            {
                selectedFileTb.Text = string.Empty;
                mediaPlayer.Source = null;
            }
        }

        private void PlayerBtnClick(object sender, RoutedEventArgs e)
        {
            string file = selectedFileTb.Text;
            if (string.IsNullOrEmpty(file))
            {
                MessageBox.Show("提示", "未选择视频文件", MessageBoxButton.OK);
                return;
            }
            mediaPlayer.Play();
        }

        private void PauseBtnClick(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void VideoPlayerWinClosed(object sender, EventArgs e)
        {
            mediaPlayer.Stop();
        }

    }
}