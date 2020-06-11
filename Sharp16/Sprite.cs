using SDL2;

namespace Sharp16
{
	internal class Sprite
	{
		internal const int MIN_SIZE = 8;

		internal int Size;
		internal int Palette;
		internal int Flags;
		internal byte[] Data;
		internal SDL.SDL_Rect BufferRect;
	}
}
