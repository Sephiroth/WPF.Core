using System;
using System.Collections.Generic;
using System.Text;
using FFmpeg.AutoGen;

namespace WpfCore
{
    public class Util
    {
        /// <summary>
        /// System.Drawing.Imaging.PixelFormat转System.Windows.Media.PixelFormat
        /// </summary>
        /// <param name="sourceFormat"></param>
        /// <returns></returns>
        private static System.Windows.Media.PixelFormat ConvertPixelFormat(System.Drawing.Imaging.PixelFormat sourceFormat)
        {
            switch (sourceFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return System.Windows.Media.PixelFormats.Bgr24;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return System.Windows.Media.PixelFormats.Bgra32;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    return System.Windows.Media.PixelFormats.Bgr32;
                default:
                    break;
            }
            return new System.Windows.Media.PixelFormat();
        }

        public static void ReadAV()
        {

        }

    }
}