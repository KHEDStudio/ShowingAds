using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] GetBytes(this Stream stream, int offset, int length)
        {
            var buffer = new byte[length];
            var readBytes = stream.Read(buffer, offset, buffer.Length);
            if (readBytes < buffer.Length)
                buffer = buffer.Take(readBytes).ToArray();
            return buffer;
        }

        public static int ReadBytes(this Stream stream, ref byte[] buffer, int offset)
        {
            var readBytes = stream.Read(buffer, offset, buffer.Length);
            if (readBytes < buffer.Length)
                buffer = buffer.Take(readBytes).ToArray();
            return readBytes;
        }
    }
}
