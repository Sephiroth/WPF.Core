using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTKUtils
{
    public class AudioUtils
    {
        /// <summary>
        /// 定义采样率 Sample Rate
        /// </summary>
        public const int FREQ = 22050;

        /// <summary>
        /// 一次捕获音频的长度
        /// </summary>
        public const int CAP_SIZE = 2048;

        public static bool InitDevice()
        {
            ALDevice audioDev = ALC.OpenDevice(null);
            AlcError err = ALC.GetError(audioDev);
            if (err != AlcError.NoError) { return false; }
            ALContext aLContext = ALC.CreateContext(audioDev, new int[0]);
            bool makeRs = ALC.MakeContextCurrent(aLContext);
            err = ALC.GetError(audioDev);
            if (!makeRs || err != AlcError.NoError) { return false; }

            //ALCdevice* inputDevice = alcCaptureOpenDevice(NULL, FREQ, AL_FORMAT_MONO16, FREQ / 2);
            ALCaptureDevice captureDev = ALC.CaptureOpenDevice(null, FREQ, ALFormat.Mono16, FREQ / 2); // FREQ
            ALC.CaptureStart(captureDev);
            err = ALC.GetError(audioDev);
            if (err != AlcError.NoError) { return false; }

            int[] buffer = AL.GenBuffers(16);
            err = ALC.GetError(audioDev);
            if (err != AlcError.NoError) { return false; }

            return true;
        }

    }
}