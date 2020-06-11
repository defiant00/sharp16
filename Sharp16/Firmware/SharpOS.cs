using SDL2;
using System;

namespace Sharp16.Firmware
{
	internal class SharpOS
	{
		private bool _inEditor;

		private Cartridge _cart;
		private IntPtr _renderer;
		private IntPtr _fontTexture;

		internal SharpOS(string filename, IntPtr renderer, IntPtr effectsBuffer, IntPtr font)
		{
			_renderer = renderer;
			_fontTexture = SDL.SDL_CreateTextureFromSurface(renderer, font);

			_cart = new Cartridge(filename, renderer, effectsBuffer, font);
		}

		internal void Draw()
		{
			if (_inEditor)
			{
				SDL.SDL_SetRenderTarget(_renderer, IntPtr.Zero);
				SDL.SDL_RenderSetClipRect(_renderer, IntPtr.Zero);
				SDL.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);
				SDL.SDL_RenderClear(_renderer);
				Text.Draw(_renderer, _fontTexture, "In editor...", 2, 2, new Color(31, 31, 31, 1));
				SDL.SDL_RenderPresent(_renderer);
			}
			else { _cart.Game.Draw(); }
		}

		internal void SaveCart() => _cart.Save();

		internal void ToggleEditor() => _inEditor = !_inEditor;

		internal void Update()
		{
			if (_inEditor) { }
			else { _cart.Game.Update(); }
		}

		internal void Unload()
		{
			SDL.SDL_DestroyTexture(_fontTexture);
		}
	}
}
