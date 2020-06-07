using SDL2;
using System;
using System.Collections.Generic;
using System.Text;

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
		public virtual bool LoadFont => true;
		public virtual void DrawEffects() { }
		public virtual void DrawBase() { }
		public virtual void DrawTop() { }
		public virtual void Update() { }

		public Vector2 Camera;
		public Inputs[] Input = new Inputs[Sharp16.PLAYER_COUNT];

		internal IntPtr _renderer;
		internal IntPtr _font;
		internal IntPtr _effectsBuffer;
		internal IntPtr _spriteBuffer;

		private List<Color[]> _palettes = new List<Color[]>();
		internal List<Sprite> _sprites = new List<Sprite>();
		private Color _drawColor;

		private const int DATA_LINE_LENGTH = 80;
		internal string _compressedPalettes
		{
			get
			{
				var bp = new BitPacker();
				bp.Pack((uint)_palettes.Count, 32);
				for (int p = 0; p < _palettes.Count; p++)
				{
					for (int i = 0; i < 16; i++)
					{
						bp.Pack((uint)_palettes[p][i].R / 8, 5);
						bp.Pack((uint)_palettes[p][i].G / 8, 5);
						bp.Pack((uint)_palettes[p][i].B / 8, 5);
						bp.Pack((uint)_palettes[p][i].A / 255, 1);
					}
				}
				return SplitToLineLength(bp.CompressedBase64, DATA_LINE_LENGTH);
			}
			set
			{
				_palettes.Clear();
				var bp = new BitPacker(value);
				uint count = bp.Unpack(32);
				for (uint p = 0; p < count; p++)
				{
					var pal = new Color[16];
					for (int i = 0; i < 16; i++)
					{
						byte r = (byte)bp.Unpack(5);
						byte g = (byte)bp.Unpack(5);
						byte b = (byte)bp.Unpack(5);
						byte a = (byte)bp.Unpack(1);
						pal[i] = new Color(r, g, b, a);
					}
					_palettes.Add(pal);
				}
			}
		}

		internal string _compressedSprites
		{
			get
			{
				return "spr";
			}
			set { }
		}

		internal string _compressedMaps
		{
			get
			{
				return "maps";
			}
			set { }
		}

		private Dictionary<char, SDL.SDL_Rect> _glyphs = new Dictionary<char, SDL.SDL_Rect>
		{
			{'\t', new SDL.SDL_Rect{w = 16}},
			{' ', new SDL.SDL_Rect{w = 4}},
			{'!', new SDL.SDL_Rect{x = 1, y = 1, w = 1, h = 11}},
			{'"', new SDL.SDL_Rect{x = 3, y = 1, w = 3, h = 11}},
			{'#', new SDL.SDL_Rect{x = 7, y = 1, w = 8, h = 11}},
			{'$', new SDL.SDL_Rect{x = 16, y = 1, w = 5, h = 11}},
			{'%', new SDL.SDL_Rect{x = 22, y = 1, w = 7, h = 11}},
			{'&', new SDL.SDL_Rect{x = 30, y = 1, w = 8, h = 11}},
			{'\'', new SDL.SDL_Rect{x = 39, y = 1, w = 1, h = 11}},
			{'(', new SDL.SDL_Rect{x = 41, y = 1, w = 3, h = 11}},
			{')', new SDL.SDL_Rect{x = 45, y = 1, w = 3, h = 11}},
			{'*', new SDL.SDL_Rect{x = 49, y = 1, w = 5, h = 11}},
			{'+', new SDL.SDL_Rect{x = 55, y = 1, w = 5, h = 11}},
			{',', new SDL.SDL_Rect{x = 61, y = 1, w = 2, h = 13}},
			{'-', new SDL.SDL_Rect{x = 64, y = 1, w = 5, h = 11}},
			{'.', new SDL.SDL_Rect{x = 70, y = 1, w = 1, h = 11}},
			{'/', new SDL.SDL_Rect{x = 72, y = 1, w = 5, h = 11}},
			{'0', new SDL.SDL_Rect{x = 78, y = 1, w = 5, h = 11}},
			{'1', new SDL.SDL_Rect{x = 84, y = 1, w = 3, h = 11}},
			{'2', new SDL.SDL_Rect{x = 88, y = 1, w = 5, h = 11}},
			{'3', new SDL.SDL_Rect{x = 94, y = 1, w = 5, h = 11}},
			{'4', new SDL.SDL_Rect{x = 100, y = 1, w = 5, h = 11}},
			{'5', new SDL.SDL_Rect{x = 106, y = 1, w = 5, h = 11}},
			{'6', new SDL.SDL_Rect{x = 112, y = 1, w = 5, h = 11}},
			{'7', new SDL.SDL_Rect{x = 118, y = 1, w = 6, h = 11}},
			{'8', new SDL.SDL_Rect{x = 1, y = 15, w = 5, h = 11}},
			{'9', new SDL.SDL_Rect{x = 7, y = 15, w = 5, h = 11}},
			{':', new SDL.SDL_Rect{x = 13, y = 15, w = 1, h = 11}},
			{';', new SDL.SDL_Rect{x = 15, y = 15, w = 2, h = 11}},
			{'<', new SDL.SDL_Rect{x = 18, y = 15, w = 3, h = 11}},
			{'=', new SDL.SDL_Rect{x = 22, y = 15, w = 4, h = 11}},
			{'>', new SDL.SDL_Rect{x = 27, y = 15, w = 3, h = 11}},
			{'?', new SDL.SDL_Rect{x = 31, y = 15, w = 5, h = 11}},
			{'@', new SDL.SDL_Rect{x = 37, y = 15, w = 7, h = 11}},
			{'A', new SDL.SDL_Rect{x = 45, y = 15, w = 7, h = 11}},
			{'B', new SDL.SDL_Rect{x = 53, y = 15, w = 5, h = 11}},
			{'C', new SDL.SDL_Rect{x = 59, y = 15, w = 6, h = 11}},
			{'D', new SDL.SDL_Rect{x = 66, y = 15, w = 5, h = 11}},
			{'E', new SDL.SDL_Rect{x = 72, y = 15, w = 5, h = 11}},
			{'F', new SDL.SDL_Rect{x = 78, y = 15, w = 5, h = 11}},
			{'G', new SDL.SDL_Rect{x = 84, y = 15, w = 6, h = 11}},
			{'H', new SDL.SDL_Rect{x = 91, y = 15, w = 5, h = 11}},
			{'I', new SDL.SDL_Rect{x = 97, y = 15, w = 5, h = 11}},
			{'J', new SDL.SDL_Rect{x = 103, y = 15, w = 5, h = 11}},
			{'K', new SDL.SDL_Rect{x = 109, y = 15, w = 5, h = 11}},
			{'L', new SDL.SDL_Rect{x = 115, y = 15, w = 5, h = 11}},
			{'M', new SDL.SDL_Rect{x = 1, y = 27, w = 7, h = 11}},
			{'N', new SDL.SDL_Rect{x = 9, y = 27, w = 5, h = 11}},
			{'O', new SDL.SDL_Rect{x = 15, y = 27, w = 7, h = 11}},
			{'P', new SDL.SDL_Rect{x = 23, y = 27, w = 5, h = 11}},
			{'Q', new SDL.SDL_Rect{x = 29, y = 27, w = 7, h = 11}},
			{'R', new SDL.SDL_Rect{x = 37, y = 27, w = 5, h = 11}},
			{'S', new SDL.SDL_Rect{x = 43, y = 27, w = 5, h = 11}},
			{'T', new SDL.SDL_Rect{x = 49, y = 27, w = 5, h = 11}},
			{'U', new SDL.SDL_Rect{x = 55, y = 27, w = 5, h = 11}},
			{'V', new SDL.SDL_Rect{x = 61, y = 27, w = 7, h = 11}},
			{'W', new SDL.SDL_Rect{x = 69, y = 27, w = 9, h = 11}},
			{'X', new SDL.SDL_Rect{x = 79, y = 27, w = 5, h = 11}},
			{'Y', new SDL.SDL_Rect{x = 85, y = 27, w = 5, h = 11}},
			{'Z', new SDL.SDL_Rect{x = 91, y = 27, w = 5, h = 11}},
			{'[', new SDL.SDL_Rect{x = 97, y = 27, w = 3, h = 11}},
			{'\\', new SDL.SDL_Rect{x = 101, y = 27, w = 5, h = 11}},
			{']', new SDL.SDL_Rect{x = 107, y = 27, w = 3, h = 11}},
			{'^', new SDL.SDL_Rect{x = 111, y = 27, w = 5, h = 11}},
			{'_', new SDL.SDL_Rect{x = 117, y = 27, w = 5, h = 11}},
			{'`', new SDL.SDL_Rect{x = 123, y = 27, w = 2, h = 11}},
			{'a', new SDL.SDL_Rect{x = 1, y = 39, w = 5, h = 11}},
			{'b', new SDL.SDL_Rect{x = 7, y = 39, w = 5, h = 11}},
			{'c', new SDL.SDL_Rect{x = 13, y = 39, w = 5, h = 11}},
			{'d', new SDL.SDL_Rect{x = 19, y = 39, w = 5, h = 11}},
			{'e', new SDL.SDL_Rect{x = 25, y = 39, w = 5, h = 11}},
			{'f', new SDL.SDL_Rect{x = 31, y = 39, w = 4, h = 11}},
			{'g', new SDL.SDL_Rect{x = 36, y = 39, w = 5, h = 15}},
			{'h', new SDL.SDL_Rect{x = 42, y = 39, w = 5, h = 11}},
			{'i', new SDL.SDL_Rect{x = 48, y = 39, w = 1, h = 11}},
			{'j', new SDL.SDL_Rect{x = 50, y = 39, w = 4, h = 15}},
			{'k', new SDL.SDL_Rect{x = 55, y = 39, w = 4, h = 11}},
			{'l', new SDL.SDL_Rect{x = 60, y = 39, w = 1, h = 11}},
			{'m', new SDL.SDL_Rect{x = 62, y = 39, w = 7, h = 11}},
			{'n', new SDL.SDL_Rect{x = 70, y = 39, w = 5, h = 11}},
			{'o', new SDL.SDL_Rect{x = 76, y = 39, w = 5, h = 11}},
			{'p', new SDL.SDL_Rect{x = 82, y = 39, w = 5, h = 15}},
			{'q', new SDL.SDL_Rect{x = 88, y = 39, w = 5, h = 15}},
			{'r', new SDL.SDL_Rect{x = 94, y = 39, w = 5, h = 11}},
			{'s', new SDL.SDL_Rect{x = 100, y = 39, w = 5, h = 11}},
			{'t', new SDL.SDL_Rect{x = 106, y = 39, w = 5, h = 11}},
			{'u', new SDL.SDL_Rect{x = 112, y = 39, w = 5, h = 11}},
			{'v', new SDL.SDL_Rect{x = 118, y = 39, w = 5, h = 11}},
			{'w', new SDL.SDL_Rect{x = 1, y = 55, w = 9, h = 11}},
			{'x', new SDL.SDL_Rect{x = 11, y = 55, w = 5, h = 11}},
			{'y', new SDL.SDL_Rect{x = 17, y = 55, w = 7, h = 15}},
			{'z', new SDL.SDL_Rect{x = 25, y = 55, w = 5, h = 11}},
			{'{', new SDL.SDL_Rect{x = 31, y = 55, w = 3, h = 11}},
			{'|', new SDL.SDL_Rect{x = 35, y = 55, w = 1, h = 11}},
			{'}', new SDL.SDL_Rect{x = 37, y = 55, w = 3, h = 11}},
			{'~', new SDL.SDL_Rect{x = 41, y = 55, w = 6, h = 11}},
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

		internal void BuildSpriteBuffer()
		{
			var packer = new TexturePacker();
			SDL.SDL_DestroyTexture(_spriteBuffer);
			var sprSurface = SDL.SDL_CreateRGBSurfaceWithFormat(0, Sharp16.SPRITE_BUFFER_SIZE, Sharp16.SPRITE_BUFFER_SIZE, 32, SDL.SDL_PIXELFORMAT_ARGB8888);
			SDL.SDL_BlitSurface(_font, IntPtr.Zero, sprSurface, IntPtr.Zero);
			packer.Add(Sharp16.FONT_SIZE);

			foreach (var s in _sprites)
			{
				var pos = packer.Add(s.Size);
				s.BufferRect = new SDL.SDL_Rect { x = pos.X, y = pos.Y, w = s.Size, h = s.Size };

				byte[] buffer = new byte[s.Size * s.Size * 4];
				int i = 0;
				foreach (byte ind in s.Data)
				{
					var c = _palettes[s.Palette][ind];
					buffer[i++] = c.B;
					buffer[i++] = c.G;
					buffer[i++] = c.R;
					buffer[i++] = c.A;
				}
				unsafe
				{
					fixed (byte* p = buffer)
					{
						var sprite = SDL.SDL_CreateRGBSurfaceWithFormatFrom((IntPtr)p, s.Size, s.Size, 32, 4 * s.Size, SDL.SDL_PIXELFORMAT_ARGB8888);
						SDL.SDL_BlitSurface(sprite, IntPtr.Zero, sprSurface, ref s.BufferRect);
						SDL.SDL_FreeSurface(sprite);
					}
				}
			}

			_spriteBuffer = SDL.SDL_CreateTextureFromSurface(_renderer, sprSurface);
			SDL.SDL_FreeSurface(sprSurface);
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
			var s = _sprites[sprite];
			var destRect = new SDL.SDL_Rect { x = x - Camera.X, y = y - Camera.Y, w = s.BufferRect.w, h = s.BufferRect.h };
			SDL.SDL_RenderCopy(_renderer, _spriteBuffer, ref s.BufferRect, ref destRect);
		}

		public void DrawText(string text, int x, int y)
		{
			SDL.SDL_SetTextureColorMod(_spriteBuffer, _drawColor.R, _drawColor.G, _drawColor.B);
			SDL.SDL_SetTextureAlphaMod(_spriteBuffer, _drawColor.A);

			int curX = x - Camera.X;
			int curY = y - Camera.Y;
			foreach (char c in text)
			{
				if (_glyphs.ContainsKey(c))
				{
					var g = _glyphs[c];
					if (g.h > 0)
					{
						var destRect = new SDL.SDL_Rect { x = curX, y = curY, w = g.w, h = g.h };
						SDL.SDL_RenderCopy(_renderer, _spriteBuffer, ref g, ref destRect);
					}
					curX += g.w + 1;
				}
			}

			SDL.SDL_SetTextureColorMod(_spriteBuffer, 255, 255, 255);
			SDL.SDL_SetTextureAlphaMod(_spriteBuffer, 255);
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
			int width = 0;
			foreach (char c in text)
			{
				if (_glyphs.ContainsKey(c))
				{
					width += _glyphs[c].w + 1;
				}
			}
			return width > 0 ? width - 1 : 0;
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

		private string SplitToLineLength(string val, int length)
		{
			string remaining = val;
			var sb = new StringBuilder();
			while (remaining.Length > length)
			{
				sb.AppendLine(remaining.Substring(0, length));
				remaining = remaining.Substring(length);
			}
			sb.Append(remaining);
			return sb.ToString();
		}

		internal void Unload()
		{
			SDL.SDL_DestroyTexture(_spriteBuffer);
		}
	}
}
