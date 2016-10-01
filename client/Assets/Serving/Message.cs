using System;
using System.IO;
using System.IO.Compression;

namespace Serving
{
    static class StreamUtils {
        public static void CopyTo(this Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024]; // Fairly arbitrary size
            int bytesRead;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }
    }

    class Compressor
    {
        public byte[] Compress(byte[] data)
        {
            using (var outStream = new MemoryStream())
            {
                using (var tinyStream = new GZipStream(outStream, CompressionMode.Compress))
                using (var mStream = new MemoryStream(data))
                    StreamUtils.CopyTo(mStream, outStream);
                return outStream.ToArray();
            }
        }
    }

    // :(
    class Decompressor
    {
        public byte[] Uncompress(byte[] data)
        {
            using (var inStream = new MemoryStream(data))
            using (var bigStream = new GZipStream(inStream, CompressionMode.Decompress))
            using (var bigStreamOut = new MemoryStream())
            {
                StreamUtils.CopyTo(bigStream, bigStreamOut);
                return bigStreamOut.ToArray();
            }
        }
    }

    public class Message
    {
        public byte[] Data { get; private set; }
        public String Type { get; private set; }
        public bool Compressed { get; private set; }

        public Message(byte[] data, String type, bool compressed)
        {
            Type = type;
            Compressed = compressed;
            Data = compressed ? new Compressor().Compress(data) : data;
        }
    }
}
