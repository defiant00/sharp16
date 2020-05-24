using SDL2;
using System;

namespace Sharp16
{
	class Graphics
	{
		IntPtr _renderer;
		Cartridge _cart;

		public Graphics(IntPtr renderer, Cartridge cart)
		{
			_renderer = renderer;
			_cart = cart;
		}

		public void Clear(int palette, int color)
		{
			SetColor(palette, color);
			SDL.SDL_RenderClear(_renderer);
		}

		public void DrawLine(int x1, int y1, int x2, int y2, int palette, int color)
		{
			SetColor(palette, color);
			SDL.SDL_RenderDrawLine(_renderer, x1, y1, x2, y2);
		}

		public void DrawPoint(int x, int y, int palette, int color)
		{
			SetColor(palette, color);
			SDL.SDL_RenderDrawPoint(_renderer, x, y);
		}

		public void DrawRect(int x, int y, int w, int h, int palette, int color)
		{
			SetColor(palette, color);
			var rect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h };
			SDL.SDL_RenderDrawRect(_renderer, ref rect);
		}

		public void FillRect(int x, int y, int w, int h, int palette, int color)
		{
			SetColor(palette, color);
			var rect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h };
			SDL.SDL_RenderFillRect(_renderer, ref rect);
		}

		void SetColor(int palette, int color)
		{
			var c = _cart.Palettes[palette][color];
			SDL.SDL_SetRenderDrawColor(_renderer, c.R, c.G, c.B, c.A);
		}
	}
}
