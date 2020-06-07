using System.IO;
using System.IO.Compression;

namespace Sharp16
{
	public static class Compression
	{
		public static byte[] Compress(byte[] input)
		{
			using var inStream = new MemoryStream(input);
			using var outStream = new MemoryStream();
			using (var comp = new DeflateStream(outStream, CompressionMode.Compress))
			{
				inStream.CopyTo(comp);
			}
			return outStream.ToArray();
		}

		public static byte[] Decompress(byte[] input)
		{
			using var inStream = new MemoryStream(input);
			using var outStream = new MemoryStream();
			using (var comp = new DeflateStream(inStream, CompressionMode.Decompress))
			{
				comp.CopyTo(outStream);
			}
			return outStream.ToArray();
		}
	}
}
