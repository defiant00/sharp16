using System;
using System.Collections.Generic;
using System.Text;

namespace Sharp16
{
	public class BitPacker
	{
		private const int SIZE = 32;
		private List<uint> _data = new List<uint>();
		private int _writeRemaining = 0;
		private int _readIndex = 0;
		private int _readRemaining = SIZE;

		private byte[] ByteArray
		{
			get
			{
				byte[] bArray = new byte[_data.Count * 4];
				uint[] iArray = _data.ToArray();
				Buffer.BlockCopy(iArray, 0, bArray, 0, bArray.Length);
				return bArray;
			}
		}

		public string CompressedBase64 => Convert.ToBase64String(Compression.Compress(ByteArray));

		public BitPacker() { }

		public BitPacker(string compressedBase64)
		{
			byte[] data = Compression.Decompress(Convert.FromBase64String(compressedBase64));
			uint[] iArray = new uint[(data.Length + 3) / 4];
			Buffer.BlockCopy(data, 0, iArray, 0, data.Length);
			_data.AddRange(iArray);
		}

		public void Pack(uint val, int size)
		{
			uint trimmed = Trim(val, size);
			if (_writeRemaining == 0)
			{
				_writeRemaining = SIZE - size;
				_data.Add(trimmed << _writeRemaining);
			}
			else if (_writeRemaining >= size)
			{
				_writeRemaining -= size;
				_data[_data.Count - 1] += trimmed << _writeRemaining;
			}
			else
			{
				_data[_data.Count - 1] += trimmed >> (size - _writeRemaining);
				_writeRemaining += SIZE - size;
				_data.Add(trimmed << _writeRemaining);
			}
		}

		private uint Trim(uint val, int size) => val << (SIZE - size) >> (SIZE - size);

		public uint Unpack(int size)
		{
			int bIndex = SIZE - _readRemaining;
			uint val = _data[_readIndex] << bIndex >> (SIZE - size);
			if (size > _readRemaining)
			{
				val += _data[_readIndex + 1] >> (SIZE - (size - _readRemaining));
			}

			_readRemaining -= size;
			if (_readRemaining <= 0)
			{
				_readRemaining += SIZE;
				_readIndex++;
			}

			return val;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach (uint i in _data)
			{
				sb.Append(Convert.ToString(i, 2).PadLeft(SIZE, '0'));
				sb.Append("|");
			}
			return sb.ToString();
		}
	}
}
