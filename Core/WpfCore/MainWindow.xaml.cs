using OpenCvSharp;
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

namespace WpfCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private readonly ArrayPool<byte> _arrayPool = ArrayPool<byte>.Create();
        OpenCvSharp.Window win = null;
        private byte[] _buffer;
        private WriteableBitmap _bitmap;
        private Int32Rect _rect;
        private CancellationTokenSource tokenSource = null;
        private bool _stopPlay = false;

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
        }

        private void PlayStreamBtnClick(object sender, RoutedEventArgs e)
        {
            string url = urlTb.Text;
            if (string.IsNullOrEmpty(url)) { return; }
            playStreamBtn.IsEnabled = false;
            tokenSource = new CancellationTokenSource();
            _stopPlay = false;
            win = new OpenCvSharp.Window();
            Task.Factory.StartNew(() =>
            {
                using (VideoCapture capture = new VideoCapture(url))
                {
                    FourCC fourCC = FourCC.FromFourChars('m', 'p', '4', 'v');
                    if (!capture.IsOpened())
                    {
                        return;
                    }
                    //double fps = capture.Get(VideoCaptureProperties.Fps);
                    //double width = capture.Get(VideoCaptureProperties.FrameWidth);
                    //double height = capture.Get(VideoCaptureProperties.FrameHeight);
                    //OpenCvSharp.Size size = new OpenCvSharp.Size(width, height);
                    using Mat frame = new Mat();
                    while (capture.Read(frame))
                    {
                        if (_stopPlay) { break; }
                        win?.ShowImage(frame);
                        //lpData = frame.Data;
                        //curSize = frame.Width * frame.Height;
                        RenderRgb(frame.Width, frame.Height, frame.DataStart, (int)frame.Total());
                    }
                    frame.Dispose();
                    capture.Dispose();
                }
                win.Close();
            }, tokenSource.Token);
        }

        private void SotpPlayStreamBtnClick(object sender, RoutedEventArgs e)
        {
            _stopPlay = true;
            playStreamBtn.IsEnabled = true;
            tokenSource?.Cancel();
            win?.Close();
        }

        private void RenderRgb(int width, int height, IntPtr dataPtr, int size)
        {
            if (_buffer == null)
            {
                _buffer = _arrayPool.Rent(size);
            }
            Marshal.Copy(dataPtr, _buffer, 0, size);
            Dispatcher.Invoke(() =>
            {
                if (_bitmap == null)
                {
                    _bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                    _rect = new Int32Rect(0, 0, width, height);
                    pImg.Source = _bitmap;
                }
                try
                {
                    _bitmap.Lock();
                    Marshal.Copy(_buffer, 0, _bitmap.BackBuffer, size);
                    _bitmap.AddDirtyRect(_rect);
                }
                catch { }
                finally { _bitmap.Unlock(); }
            });
        }

    }
}