using System;

namespace PoCBinZip
{
    class Program
    {
        static void Main(string[] args)
        {
        	byte[] buf = Decompress(compressedSC);
        	
        	return;
        }
		public static byte[] Compress(byte[] data)
        {
            var compressedStream = new MemoryStream();
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress, false))
            {
                gzipStream.Write(data, 0, data.Length);
            }

            return compressedStream.ToArray();
        }

        public static byte[] Decompress(byte[] compressedData)
        {
            var uncompressedStream = new MemoryStream();

            using (var compressedStream = new MemoryStream(compressedData))
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                gzipStream.CopyTo(uncompressedStream);
            }

            return uncompressedStream.ToArray();
        }
    }
}