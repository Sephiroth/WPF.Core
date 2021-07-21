using OpenTK.Audio.OpenAL;
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
    /// OpenALWin.xaml 的交互逻辑
    /// </summary>
    public partial class OpenALWin : Window
    {
        public OpenALWin()
        {
            InitializeComponent();
        }

        public bool Init()
        {
            ALDevice audioDev = ALC.OpenDevice(null);
            AlcError err = ALC.GetError(audioDev);
            if (err != AlcError.NoError) { return false; }
            ALContext aLContext = ALC.CreateContext(audioDev, new int[0]);
            bool makeRs = ALC.MakeContextCurrent(aLContext);
            err = ALC.GetError(audioDev);
            if (!makeRs || err != AlcError.NoError) { return false; }

            //ALCdevice* inputDevice = alcCaptureOpenDevice(NULL, FREQ, AL_FORMAT_MONO16, FREQ / 2);
            ALCaptureDevice captureDev = ALC.CaptureOpenDevice(null, 144000, ALFormat.Mono16, 144000 / 2); // FREQ
            ALC.CaptureStart(captureDev);
            err = ALC.GetError(audioDev);
            if (err != AlcError.NoError) { return false; }

            int[] buffer = AL.GenBuffers(16);
            err = ALC.GetError(audioDev);
            if (err != AlcError.NoError) { return false; }

            return true;
        }

        private void testBtnClick(object sender, RoutedEventArgs e)
        {
            Init();
        }

    }
}