using SDL2;

namespace Sharp16
{
	public class Sprite
	{
		public const int MIN_SIZE = 8;

		public int Size;
		public int Palette;
		public int Flags;
		public byte[] Data;
		public SDL.SDL_Rect BufferRect;
	}
}
