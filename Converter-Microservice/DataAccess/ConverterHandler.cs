using System.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Xabe.FFmpeg;

namespace DataAccess
{
    public class ConverterHandler
    {
        public async Task con()
        {
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo("");

            IStream? videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.mpeg4);
            IStream? audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                ?.SetCodec(AudioCodec.mp3);

            IConversionResult? obj = await FFmpeg.Conversions.New()
                .AddStream(audioStream, videoStream)
                .SetOutput(outputPath)
                .Start();
        }
    }
}
