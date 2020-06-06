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
		public virtual bool LoadFont { get => true; }

		public virtual void DrawEffects() { }
		public virtual void DrawBase() { }
		public virtual void DrawTop() { }
		public virtual void Update() { }

		public Vector2 Camera;
		public Inputs[] Input = new Inputs[Sharp16.PLAYER_COUNT];

		internal List<Color[]> _palettes = new List<Color[]>();
		internal Color _drawColor;
		internal IntPtr _renderer;
		internal IntPtr _effectsBuffer;
		internal IntPtr _sprites;

		private struct Glyph
		{
			public SDL.SDL_Rect Rect;
			public int Width;
		}
		private Dictionary<char, Glyph> _glyphs = new Dictionary<char, Glyph>
		{
			{'\t', new Glyph{Width = 16}},
			{' ', new Glyph{Width = 4}},
			{'!', new Glyph{Rect = new SDL.SDL_Rect{x = 1, y = 1, w = 1, h = 11}, Width = 2}},
			{'"', new Glyph{Rect = new SDL.SDL_Rect{x = 3, y = 1, w = 3, h = 11}, Width = 4}},
			{'#', new Glyph{Rect = new SDL.SDL_Rect{x = 7, y = 1, w = 8, h = 11}, Width = 9}},
			{'$', new Glyph{Rect = new SDL.SDL_Rect{x = 16, y = 1, w = 5, h = 11}, Width = 6}},
			{'%', new Glyph{Rect = new SDL.SDL_Rect{x = 22, y = 1, w = 7, h = 11}, Width = 8}},
			{'&', new Glyph{Rect = new SDL.SDL_Rect{x = 30, y = 1, w = 8, h = 11}, Width = 9}},
			{'\'', new Glyph{Rect = new SDL.SDL_Rect{x = 39, y = 1, w = 1, h = 11}, Width = 2}},
			{'(', new Glyph{Rect = new SDL.SDL_Rect{x = 41, y = 1, w = 3, h = 11}, Width = 4}},
			{')', new Glyph{Rect = new SDL.SDL_Rect{x = 45, y = 1, w = 3, h = 11}, Width = 4}},
			{'*', new Glyph{Rect = new SDL.SDL_Rect{x = 49, y = 1, w = 5, h = 11}, Width = 6}},
			{'+', new Glyph{Rect = new SDL.SDL_Rect{x = 55, y = 1, w = 5, h = 11}, Width = 6}},
			{',', new Glyph{Rect = new SDL.SDL_Rect{x = 61, y = 1, w = 2, h = 13}, Width = 3}},
			{'-', new Glyph{Rect = new SDL.SDL_Rect{x = 64, y = 1, w = 5, h = 11}, Width = 6}},
			{'.', new Glyph{Rect = new SDL.SDL_Rect{x = 70, y = 1, w = 1, h = 11}, Width = 2}},
			{'/', new Glyph{Rect = new SDL.SDL_Rect{x = 72, y = 1, w = 5, h = 11}, Width = 6}},
			{'0', new Glyph{Rect = new SDL.SDL_Rect{x = 78, y = 1, w = 5, h = 11}, Width = 6}},
			{'1', new Glyph{Rect = new SDL.SDL_Rect{x = 84, y = 1, w = 3, h = 11}, Width = 4}},
			{'2', new Glyph{Rect = new SDL.SDL_Rect{x = 88, y = 1, w = 5, h = 11}, Width = 6}},
			{'3', new Glyph{Rect = new SDL.SDL_Rect{x = 94, y = 1, w = 5, h = 11}, Width = 6}},
			{'4', new Glyph{Rect = new SDL.SDL_Rect{x = 100, y = 1, w = 5, h = 11}, Width = 6}},
			{'5', new Glyph{Rect = new SDL.SDL_Rect{x = 106, y = 1, w = 5, h = 11}, Width = 6}},
			{'6', new Glyph{Rect = new SDL.SDL_Rect{x = 112, y = 1, w = 5, h = 11}, Width = 6}},
			{'7', new Glyph{Rect = new SDL.SDL_Rect{x = 118, y = 1, w = 6, h = 11}, Width = 7}},
			{'8', new Glyph{Rect = new SDL.SDL_Rect{x = 1, y = 15, w = 5, h = 11}, Width = 6}},
			{'9', new Glyph{Rect = new SDL.SDL_Rect{x = 7, y = 15, w = 5, h = 11}, Width = 6}},
			{':', new Glyph{Rect = new SDL.SDL_Rect{x = 13, y = 15, w = 1, h = 11}, Width = 2}},
			{';', new Glyph{Rect = new SDL.SDL_Rect{x = 15, y = 15, w = 2, h = 11}, Width = 3}},
			{'<', new Glyph{Rect = new SDL.SDL_Rect{x = 18, y = 15, w = 3, h = 11}, Width = 4}},
			{'=', new Glyph{Rect = new SDL.SDL_Rect{x = 22, y = 15, w = 4, h = 11}, Width = 5}},
			{'>', new Glyph{Rect = new SDL.SDL_Rect{x = 27, y = 15, w = 3, h = 11}, Width = 4}},
			{'?', new Glyph{Rect = new SDL.SDL_Rect{x = 31, y = 15, w = 5, h = 11}, Width = 6}},
			{'@', new Glyph{Rect = new SDL.SDL_Rect{x = 37, y = 15, w = 7, h = 11}, Width = 8}},
			{'A', new Glyph{Rect = new SDL.SDL_Rect{x = 45, y = 15, w = 7, h = 11}, Width = 8}},
			{'B', new Glyph{Rect = new SDL.SDL_Rect{x = 53, y = 15, w = 5, h = 11}, Width = 6}},
			{'C', new Glyph{Rect = new SDL.SDL_Rect{x = 59, y = 15, w = 6, h = 11}, Width = 7}},
			{'D', new Glyph{Rect = new SDL.SDL_Rect{x = 66, y = 15, w = 5, h = 11}, Width = 6}},
			{'E', new Glyph{Rect = new SDL.SDL_Rect{x = 72, y = 15, w = 5, h = 11}, Width = 6}},
			{'F', new Glyph{Rect = new SDL.SDL_Rect{x = 78, y = 15, w = 5, h = 11}, Width = 6}},
			{'G', new Glyph{Rect = new SDL.SDL_Rect{x = 84, y = 15, w = 6, h = 11}, Width = 7}},
			{'H', new Glyph{Rect = new SDL.SDL_Rect{x = 91, y = 15, w = 5, h = 11}, Width = 6}},
			{'I', new Glyph{Rect = new SDL.SDL_Rect{x = 97, y = 15, w = 5, h = 11}, Width = 6}},
			{'J', new Glyph{Rect = new SDL.SDL_Rect{x = 103, y = 15, w = 5, h = 11}, Width = 6}},
			{'K', new Glyph{Rect = new SDL.SDL_Rect{x = 109, y = 15, w = 5, h = 11}, Width = 6}},
			{'L', new Glyph{Rect = new SDL.SDL_Rect{x = 115, y = 15, w = 5, h = 11}, Width = 6}},
			{'M', new Glyph{Rect = new SDL.SDL_Rect{x = 1, y = 27, w = 7, h = 11}, Width = 8}},
			{'N', new Glyph{Rect = new SDL.SDL_Rect{x = 9, y = 27, w = 5, h = 11}, Width = 6}},
			{'O', new Glyph{Rect = new SDL.SDL_Rect{x = 15, y = 27, w = 7, h = 11}, Width = 8}},
			{'P', new Glyph{Rect = new SDL.SDL_Rect{x = 23, y = 27, w = 5, h = 11}, Width = 6}},
			{'Q', new Glyph{Rect = new SDL.SDL_Rect{x = 29, y = 27, w = 7, h = 11}, Width = 8}},
			{'R', new Glyph{Rect = new SDL.SDL_Rect{x = 37, y = 27, w = 5, h = 11}, Width = 6}},
			{'S', new Glyph{Rect = new SDL.SDL_Rect{x = 43, y = 27, w = 5, h = 11}, Width = 6}},
			{'T', new Glyph{Rect = new SDL.SDL_Rect{x = 49, y = 27, w = 5, h = 11}, Width = 6}},
			{'U', new Glyph{Rect = new SDL.SDL_Rect{x = 55, y = 27, w = 5, h = 11}, Width = 6}},
			{'V', new Glyph{Rect = new SDL.SDL_Rect{x = 61, y = 27, w = 7, h = 11}, Width = 8}},
			{'W', new Glyph{Rect = new SDL.SDL_Rect{x = 69, y = 27, w = 9, h = 11}, Width = 10}},
			{'X', new Glyph{Rect = new SDL.SDL_Rect{x = 79, y = 27, w = 5, h = 11}, Width = 6}},
			{'Y', new Glyph{Rect = new SDL.SDL_Rect{x = 85, y = 27, w = 5, h = 11}, Width = 6}},
			{'Z', new Glyph{Rect = new SDL.SDL_Rect{x = 91, y = 27, w = 5, h = 11}, Width = 6}},
			{'[', new Glyph{Rect = new SDL.SDL_Rect{x = 97, y = 27, w = 3, h = 11}, Width = 4}},
			{'\\', new Glyph{Rect = new SDL.SDL_Rect{x = 101, y = 27, w = 5, h = 11}, Width = 6}},
			{']', new Glyph{Rect = new SDL.SDL_Rect{x = 107, y = 27, w = 3, h = 11}, Width = 4}},
			{'^', new Glyph{Rect = new SDL.SDL_Rect{x = 111, y = 27, w = 5, h = 11}, Width = 6}},
			{'_', new Glyph{Rect = new SDL.SDL_Rect{x = 117, y = 27, w = 5, h = 11}, Width = 6}},
			{'`', new Glyph{Rect = new SDL.SDL_Rect{x = 123, y = 27, w = 2, h = 11}, Width = 3}},
			{'a', new Glyph{Rect = new SDL.SDL_Rect{x = 1, y = 39, w = 5, h = 11}, Width = 6}},
			{'b', new Glyph{Rect = new SDL.SDL_Rect{x = 7, y = 39, w = 5, h = 11}, Width = 6}},
			{'c', new Glyph{Rect = new SDL.SDL_Rect{x = 13, y = 39, w = 5, h = 11}, Width = 6}},
			{'d', new Glyph{Rect = new SDL.SDL_Rect{x = 19, y = 39, w = 5, h = 11}, Width = 6}},
			{'e', new Glyph{Rect = new SDL.SDL_Rect{x = 25, y = 39, w = 5, h = 11}, Width = 6}},
			{'f', new Glyph{Rect = new SDL.SDL_Rect{x = 31, y = 39, w = 4, h = 11}, Width = 5}},
			{'g', new Glyph{Rect = new SDL.SDL_Rect{x = 36, y = 39, w = 5, h = 15}, Width = 6}},
			{'h', new Glyph{Rect = new SDL.SDL_Rect{x = 42, y = 39, w = 5, h = 11}, Width = 6}},
			{'i', new Glyph{Rect = new SDL.SDL_Rect{x = 48, y = 39, w = 1, h = 11}, Width = 2}},
			{'j', new Glyph{Rect = new SDL.SDL_Rect{x = 50, y = 39, w = 4, h = 15}, Width = 5}},
			{'k', new Glyph{Rect = new SDL.SDL_Rect{x = 55, y = 39, w = 4, h = 11}, Width = 5}},
			{'l', new Glyph{Rect = new SDL.SDL_Rect{x = 60, y = 39, w = 1, h = 11}, Width = 2}},
			{'m', new Glyph{Rect = new SDL.SDL_Rect{x = 62, y = 39, w = 7, h = 11}, Width = 8}},
			{'n', new Glyph{Rect = new SDL.SDL_Rect{x = 70, y = 39, w = 5, h = 11}, Width = 6}},
			{'o', new Glyph{Rect = new SDL.SDL_Rect{x = 76, y = 39, w = 5, h = 11}, Width = 6}},
			{'p', new Glyph{Rect = new SDL.SDL_Rect{x = 82, y = 39, w = 5, h = 15}, Width = 6}},
			{'q', new Glyph{Rect = new SDL.SDL_Rect{x = 88, y = 39, w = 5, h = 15}, Width = 6}},
			{'r', new Glyph{Rect = new SDL.SDL_Rect{x = 94, y = 39, w = 5, h = 11}, Width = 6}},
			{'s', new Glyph{Rect = new SDL.SDL_Rect{x = 100, y = 39, w = 5, h = 11}, Width = 6}},
			{'t', new Glyph{Rect = new SDL.SDL_Rect{x = 106, y = 39, w = 5, h = 11}, Width = 6}},
			{'u', new Glyph{Rect = new SDL.SDL_Rect{x = 112, y = 39, w = 5, h = 11}, Width = 6}},
			{'v', new Glyph{Rect = new SDL.SDL_Rect{x = 118, y = 39, w = 5, h = 11}, Width = 6}},
			{'w', new Glyph{Rect = new SDL.SDL_Rect{x = 1, y = 55, w = 9, h = 11}, Width = 10}},
			{'x', new Glyph{Rect = new SDL.SDL_Rect{x = 11, y = 55, w = 5, h = 11}, Width = 6}},
			{'y', new Glyph{Rect = new SDL.SDL_Rect{x = 17, y = 55, w = 7, h = 15}, Width = 8}},
			{'z', new Glyph{Rect = new SDL.SDL_Rect{x = 25, y = 55, w = 5, h = 11}, Width = 6}},
			{'{', new Glyph{Rect = new SDL.SDL_Rect{x = 31, y = 55, w = 3, h = 11}, Width = 4}},
			{'|', new Glyph{Rect = new SDL.SDL_Rect{x = 35, y = 55, w = 1, h = 11}, Width = 2}},
			{'}', new Glyph{Rect = new SDL.SDL_Rect{x = 37, y = 55, w = 3, h = 11}, Width = 4}},
			{'~', new Glyph{Rect = new SDL.SDL_Rect{x = 41, y = 55, w = 6, h = 11}, Width = 7}},
		};

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
			Clear();
		}

		public void ClearClipRect()
		{
			SDL.SDL_RenderSetClipRect(_renderer, IntPtr.Zero);
		}

		public void DrawLine(int x1, int y1, int x2, int y2)
		{
			SDL.SDL_RenderDrawLine(_renderer, x1 - Camera.X, y1 - Camera.Y, x2 - Camera.X, y2 - Camera.Y);
		}

		public void DrawLine(int x1, int y1, int x2, int y2, int palette, int color)
		{
			SetColor(palette, color);
			DrawLine(x1, y1, x2, y2);
		}

		public void DrawMap(int map, int x, int y)
		{

		}

		public void DrawPoint(int x, int y)
		{
			SDL.SDL_RenderDrawPoint(_renderer, x - Camera.X, y - Camera.Y);
		}

		public void DrawPoint(int x, int y, int palette, int color)
		{
			SetColor(palette, color);
			DrawPoint(x, y);
		}

		public void DrawRect(int x, int y, int w, int h)
		{
			var rect = new SDL.SDL_Rect { x = x - Camera.X, y = y - Camera.Y, w = w, h = h };
			SDL.SDL_RenderDrawRect(_renderer, ref rect);
		}

		public void DrawRect(int x, int y, int w, int h, int palette, int color)
		{
			SetColor(palette, color);
			DrawRect(x, y, w, h);
		}

		public void DrawSprite(int sprite, int x, int y)
		{

		}

		public void DrawText(string text, int x, int y)
		{
			SDL.SDL_SetTextureColorMod(_sprites, _drawColor.R, _drawColor.G, _drawColor.B);
			SDL.SDL_SetTextureAlphaMod(_sprites, _drawColor.A);

			int curX = x - Camera.X;
			int curY = y - Camera.Y;
			foreach (char c in text)
			{
				if (_glyphs.ContainsKey(c))
				{
					var g = _glyphs[c];
					if (g.Rect.w > 0)
					{
						var destRect = new SDL.SDL_Rect { x = curX, y = curY, w = g.Rect.w, h = g.Rect.h };
						SDL.SDL_RenderCopy(_renderer, _sprites, ref g.Rect, ref destRect);
					}
					curX += g.Width;
				}
			}

			SDL.SDL_SetTextureColorMod(_sprites, 255, 255, 255);
			SDL.SDL_SetTextureAlphaMod(_sprites, 255);
		}

		public void DrawText(string text, int x, int y, int palette, int color)
		{
			SetColor(palette, color);
			DrawText(text, x, y);
		}

		public void FillRect(int x, int y, int w, int h)
		{
			var rect = new SDL.SDL_Rect { x = x - Camera.X, y = y - Camera.Y, w = w, h = h };
			SDL.SDL_RenderFillRect(_renderer, ref rect);
		}

		public void FillRect(int x, int y, int w, int h, int palette, int color)
		{
			SetColor(palette, color);
			FillRect(x, y, w, h);
		}

		public int MeasureText(string text)
		{
			return 0;
		}

		public void SetClipRect(int x, int y, int w, int h)
		{
			var rect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h };
			SDL.SDL_RenderSetClipRect(_renderer, ref rect);
		}

		public void SetColor(int palette, int color)
		{
			_drawColor = _palettes[palette][color];
			SDL.SDL_SetRenderDrawColor(_renderer, _drawColor.R, _drawColor.G, _drawColor.B, _drawColor.A);
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
