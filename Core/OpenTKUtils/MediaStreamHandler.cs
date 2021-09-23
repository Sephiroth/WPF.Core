using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTKUtils
{
    public unsafe class MediaStreamHandler
    {
        public void OpenStream(string url)
        {
            //MediaStream stream = new MediaStream(url);
            //EmguFFmpeg.MediaReader reader = new MediaReader(url);
            //MediaPacket packet = new MediaPacket();
            //int readNum = reader.ReadPacket(packet);


        }

        static void Main() {
            MediaStreamHandler handler = new MediaStreamHandler();
            handler.OpenStream("rtmp://58.200.131.2:1935/livetv/ahtv");
        }
    }
}