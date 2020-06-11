namespace Sharp16
{
	internal struct Color
	{
		internal byte R, G, B, A;

		internal Color(byte r, byte g, byte b, byte a)
		{
			R = (byte)((r << 3) + (r >> 2));
			G = (byte)((g << 3) + (g >> 2));
			B = (byte)((b << 3) + (b >> 2));
			A = (byte)(a > 0 ? 255 : 0);
		}
	}
}
