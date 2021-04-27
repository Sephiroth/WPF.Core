using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Emgu.CV.BitmapExtension;

namespace WpfCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ArrayPool<byte> _arrayPool = ArrayPool<byte>.Create();
        OpenCvSharp.Window win = null;
        private byte[] _buffer;
        private WriteableBitmap _writeBitmap;
        /// <summary>
        /// 当前要刷新的区域
        /// </summary>
        private Int32Rect _rect;
        private CancellationTokenSource tokenSource = null;
        private bool _stopPlay = false;

        /// <summary>
        /// 当前选择的码率
        /// </summary>
        private OpenCvSharp.Size _tranformSize = new OpenCvSharp.Size(0, 0);
        /// <summary>
        /// 转换码率后的每一帧
        /// </summary>
        private OpenCvSharp.Mat _tranformFrame = new OpenCvSharp.Mat();

        public MainWindow()
        {
            InitializeComponent();
        }
        private void MainWinLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void MainWinClosed(object sender, EventArgs e)
        {
            if (_buffer != null) { _arrayPool.Return(_buffer); }
            _tranformFrame.Dispose();
        }

        private void PlayStreamBtnClick(object sender, RoutedEventArgs e)
        {
            string url = urlTb.Text;
            if (string.IsNullOrEmpty(url)) { return; }
            playStreamBtn.IsEnabled = false;
            tokenSource = new CancellationTokenSource();
            _stopPlay = false;

            Task.Factory.StartNew(() =>
            {
                OpenCvCaptureVideoStream(url, LoadImgByOpenCvMat);
                //EmguCvCaptureVideoStream(url, LoadImgByEmguCvMat);
            }, tokenSource.Token);
        }

        #region OpenCv捕获、播放视频流
        /// <summary>
        /// 基于OpenCv捕获视频流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="action"></param>
        public void OpenCvCaptureVideoStream(string url, Action<OpenCvSharp.Mat> action)
        {
            using (OpenCvSharp.VideoCapture capture = new OpenCvSharp.VideoCapture(url))
            {
                //FourCC fourCC = FourCC.FromFourChars('m', 'p', '4', 'v');
                if (!capture.IsOpened())
                {
                    return;
                }
                //double fps = capture.Get(VideoCaptureProperties.Fps);
                //double width = capture.Get(VideoCaptureProperties.FrameWidth);
                //double height = capture.Get(VideoCaptureProperties.FrameHeight);
                //OpenCvSharp.Size size = new OpenCvSharp.Size(width, height);
                using OpenCvSharp.Mat frame = new OpenCvSharp.Mat();
                while (_stopPlay == false)
                {
                    if (capture.Read(frame))
                    {
                        //win?.ShowImage(frame);
                        action?.Invoke(frame);
                    }
                }
                frame.Dispose();
                capture.Dispose();
            }
        }


        /// <summary>
        /// 基于WriteableBitmap绘制
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="dataPtr"></param>
        /// <param name="size"></param>
        private void LoadImgByOpenCvMat(OpenCvSharp.Mat frame)
        {
            // 转码率
            if (_tranformSize.Height != 0 && frame.Height != _tranformSize.Height)
            {
                OpenCvSharp.Cv2.Resize(frame, _tranformFrame, _tranformSize);
                frame = _tranformFrame;
            }
            if (_rect.Height != frame.Height)
            {
                _rect = new Int32Rect(0, 0, frame.Width, frame.Height);
            }

            using System.Drawing.Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame);
            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Dispatcher.Invoke(() =>
            {
                if (_writeBitmap == null)
                {
                    _writeBitmap = new WriteableBitmap(frame.Width, frame.Height, 96, 96, PixelFormats.Bgra32, null); //BitmapPalettes.Gray256
                    pImg.Source = _writeBitmap;
                    _rect = new Int32Rect(0, 0, frame.Width, frame.Height);
                }
                _writeBitmap.Lock();
                _writeBitmap.WritePixels(_rect, data.Scan0, (4 * data.Width * data.Height), data.Stride);
                _writeBitmap.Unlock();
            });
            bitmap.UnlockBits(data);
            bitmap.Dispose();
        }
        #endregion

        #region EmguCv加载视频流播放
        public void EmguCvCaptureVideoStream(string url, Action<Emgu.CV.Mat> action)
        {
            using Emgu.CV.VideoCapture capture = new Emgu.CV.VideoCapture(url, Emgu.CV.VideoCapture.API.Any);
            if (capture.IsOpened == false)
            {
                return;
            }
            using Emgu.CV.Mat frame = capture.QueryFrame();
            while (_stopPlay == false)
            {
                if (capture.Read(frame)) //capture.Retrieve(frame)// 无法继续读取
                {
                    if (!frame.IsEmpty)
                    {
                        action?.Invoke(frame);
                    }
                }
            }
            frame.Dispose();
            capture.Dispose();
        }

        public void LoadImgByEmguCvMat(Emgu.CV.Mat frame)
        {
            using System.Drawing.Bitmap bitmap = frame.ToBitmap();
            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Dispatcher.Invoke(() =>
            {
                if (_writeBitmap == null)
                {
                    _writeBitmap = new WriteableBitmap(frame.Width, frame.Height, 96, 96, PixelFormats.Bgra32, null);
                    pImg.Source = _writeBitmap;
                    _rect = new Int32Rect(0, 0, bitmap.Width, bitmap.Height);
                }
                _writeBitmap.Lock();
                _writeBitmap.WritePixels(_rect, data.Scan0, (4 * data.Width * data.Height), data.Stride);
                //Marshal.Copy(data.Scan0,_writeBitmap.BackBuffer,0,1);
                //_writeBitmap.AddDirtyRect(rec);
                _writeBitmap.Unlock();
            });
            bitmap.UnlockBits(data);
            bitmap.Dispose();
        }
        #endregion

        private void SotpPlayStreamBtnClick(object sender, RoutedEventArgs e)
        {
            _stopPlay = true;
            _writeBitmap = null;
            playStreamBtn.IsEnabled = true;
            tokenSource?.Cancel();
            win?.Close();
        }

        private void openWinBtnClick(object sender, RoutedEventArgs e)
        {
            VideoPlayerWin win = new VideoPlayerWin();
            _ = win.ShowDialog();
        }

        private void RbChanged(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            if (radio == null) { return; }
            string text = radio.Content as string;
            if (string.Equals(text, "原画")) { _tranformSize = new OpenCvSharp.Size(0, 0); }
            else if (string.Equals(text, "1080p")) { _tranformSize = new OpenCvSharp.Size(1920, 1080); }
            else if (string.Equals(text, "720p")) { _tranformSize = new OpenCvSharp.Size(1280, 720); }
            else if (string.Equals(text, "480p")) { _tranformSize = new OpenCvSharp.Size(854, 480); }
        }
    }
}