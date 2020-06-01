using SDL2;
using System;
using System.Collections.Generic;

namespace Sharp16
{
	public enum BlendMode
	{
		Add,
		Alpha,
		Average,
		Multiply,
	}

	public class SharpGame
	{
		public virtual void DrawEffects() { }
		public virtual void DrawBase() { }
		public virtual void DrawTop() { }
		public virtual void Update() { }

		public Inputs Input;

		internal List<Color[]> _palettes = new List<Color[]>();
		internal IntPtr _renderer;
		internal IntPtr _effectsBuffer;

		internal void Draw()
		{
			SDL.SDL_SetRenderTarget(_renderer, _effectsBuffer);
			SDL.SDL_RenderSetClipRect(_renderer, IntPtr.Zero);
			SDL.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 0);
			SDL.SDL_RenderClear(_renderer);
			DrawEffects();
			SDL.SDL_SetRenderTarget(_renderer, IntPtr.Zero);
			SDL.SDL_RenderSetClipRect(_renderer, IntPtr.Zero);
			SDL.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);
			SDL.SDL_RenderClear(_renderer);
			DrawBase();
			SDL.SDL_RenderSetClipRect(_renderer, IntPtr.Zero);
			SDL.SDL_RenderCopy(_renderer, _effectsBuffer, IntPtr.Zero, IntPtr.Zero);
			DrawTop();
			SDL.SDL_RenderPresent(_renderer);
		}

		public void Clear()
		{
			SDL.SDL_RenderFillRect(_renderer, IntPtr.Zero);
		}

		public void Clear(int palette, int color)
		{
			SetColor(palette, color);
			SDL.SDL_RenderFillRect(_renderer, IntPtr.Zero);
		}

		public void ClearClipRect()
		{
			SDL.SDL_RenderSetClipRect(_renderer, IntPtr.Zero);
		}

		public void DrawLine(int x1, int y1, int x2, int y2)
		{
			SDL.SDL_RenderDrawLine(_renderer, x1, y1, x2, y2);
		}

		public void DrawLine(int x1, int y1, int x2, int y2, int palette, int color)
		{
			SetColor(palette, color);
			SDL.SDL_RenderDrawLine(_renderer, x1, y1, x2, y2);
		}

		public void DrawPoint(int x, int y)
		{
			SDL.SDL_RenderDrawPoint(_renderer, x, y);
		}

		public void DrawPoint(int x, int y, int palette, int color)
		{
			SetColor(palette, color);
			SDL.SDL_RenderDrawPoint(_renderer, x, y);
		}

		public void DrawRect(int x, int y, int w, int h)
		{
			var rect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h };
			SDL.SDL_RenderDrawRect(_renderer, ref rect);
		}

		public void DrawRect(int x, int y, int w, int h, int palette, int color)
		{
			SetColor(palette, color);
			var rect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h };
			SDL.SDL_RenderDrawRect(_renderer, ref rect);
		}

		public void DrawMap(int x, int y)
		{

		}

		public void DrawSprite(int x, int y)
		{

		}

		public void FillRect(int x, int y, int w, int h)
		{
			var rect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h };
			SDL.SDL_RenderFillRect(_renderer, ref rect);
		}

		public void FillRect(int x, int y, int w, int h, int palette, int color)
		{
			SetColor(palette, color);
			var rect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h };
			SDL.SDL_RenderFillRect(_renderer, ref rect);
		}

		public void SetClipRect(int x, int y, int w, int h)
		{
			var rect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h };
			SDL.SDL_RenderSetClipRect(_renderer, ref rect);
		}

		public void SetColor(int palette, int color)
		{
			var c = _palettes[palette][color];
			SDL.SDL_SetRenderDrawColor(_renderer, c.R, c.G, c.B, c.A);
		}

		public void SetEffects(BlendMode mode)
		{
			switch (mode)
			{
				case BlendMode.Add:
					SDL.SDL_SetTextureAlphaMod(_effectsBuffer, 255);
					SDL.SDL_SetTextureBlendMode(_effectsBuffer, SDL.SDL_BlendMode.SDL_BLENDMODE_ADD);
					break;
				case BlendMode.Alpha:
					SDL.SDL_SetTextureAlphaMod(_effectsBuffer, 255);
					SDL.SDL_SetTextureBlendMode(_effectsBuffer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
					break;
				case BlendMode.Average:
					SDL.SDL_SetTextureAlphaMod(_effectsBuffer, 127);
					SDL.SDL_SetTextureBlendMode(_effectsBuffer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
					break;
				case BlendMode.Multiply:
					SDL.SDL_SetTextureAlphaMod(_effectsBuffer, 255);
					SDL.SDL_SetTextureBlendMode(_effectsBuffer, SDL.SDL_BlendMode.SDL_BLENDMODE_MUL);
					break;
			}
		}
	}
}
