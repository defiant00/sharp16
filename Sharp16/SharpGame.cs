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

		protected Inputs[] Input => Sharp16._input;
		protected Vector2 Camera;

		internal IntPtr _renderer;
		internal IntPtr _fontSurface;
		internal IntPtr _effectsBuffer;
		internal IntPtr _spriteBuffer;

		private List<Color[]> _palettes = new List<Color[]>();
		private List<Sprite> _sprites = new List<Sprite>();
		private Color _drawColor;

		private const int DATA_LINE_LENGTH = 80;

		internal string CompressedPalettes
		{
			get
			{
				if (_palettes.Count > 0)
				{
					var bp = new BitPacker();
					bp.Pack(_palettes.Count, 32);
					foreach (var palette in _palettes)
					{
						foreach (var color in palette)
						{
							bp.Pack(color.R / 8, 5);
							bp.Pack(color.G / 8, 5);
							bp.Pack(color.B / 8, 5);
							bp.Pack(color.A / 255, 1);
						}
					}
					return SplitToLineLength(bp.CompressedBase64, DATA_LINE_LENGTH);
				}
				return null;
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

		internal string CompressedSprites
		{
			get
			{
				if (_sprites.Count > 0)
				{
					var bp = new BitPacker();
					bp.Pack(_sprites.Count, 32);
					foreach (var sprite in _sprites)
					{
						bp.Pack(sprite.Size, 8);
						bp.Pack(sprite.Palette, 16);
						bp.Pack(sprite.Flags, 16);
						foreach (byte b in sprite.Data)
						{
							bp.Pack(b, 4);
						}
					}
					return SplitToLineLength(bp.CompressedBase64, DATA_LINE_LENGTH);
				}
				return null;
			}
			set
			{
				_sprites.Clear();
				var bp = new BitPacker(value);
				uint count = bp.Unpack(32);
				for (uint s = 0; s < count; s++)
				{
					var sprite = new Sprite
					{
						Size = (int)bp.Unpack(8),
						Palette = (int)bp.Unpack(16),
						Flags = (int)bp.Unpack(16)
					};
					sprite.Data = new byte[sprite.Size * sprite.Size];
					for (int i = 0; i < sprite.Data.Length; i++)
					{
						sprite.Data[i] = (byte)bp.Unpack(4);
					}
					_sprites.Add(sprite);
				}
			}
		}

		internal string CompressedMaps
		{
			get
			{
				return "maps";
			}
			set { }
		}

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

			if (LoadFont)
			{
				SDL.SDL_BlitSurface(_fontSurface, IntPtr.Zero, sprSurface, IntPtr.Zero);
				packer.Add(Sharp16.FONT_SIZE);
			}

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

		protected void Clear()
		{
			SDL.SDL_RenderFillRect(_renderer, IntPtr.Zero);
		}

		protected void Clear(int palette, int color)
		{
			SetColor(palette, color);
			Clear();
		}

		protected void ClearClipRect()
		{
			SDL.SDL_RenderSetClipRect(_renderer, IntPtr.Zero);
		}

		protected void DrawLine(int x1, int y1, int x2, int y2)
		{
			SDL.SDL_RenderDrawLine(_renderer, x1 - Camera.X, y1 - Camera.Y, x2 - Camera.X, y2 - Camera.Y);
		}

		protected void DrawLine(int x1, int y1, int x2, int y2, int palette, int color)
		{
			SetColor(palette, color);
			DrawLine(x1, y1, x2, y2);
		}

		protected void DrawMap(int map, int x, int y)
		{

		}

		protected void DrawPoint(int x, int y)
		{
			SDL.SDL_RenderDrawPoint(_renderer, x - Camera.X, y - Camera.Y);
		}

		protected void DrawPoint(int x, int y, int palette, int color)
		{
			SetColor(palette, color);
			DrawPoint(x, y);
		}

		protected void DrawRect(int x, int y, int w, int h)
		{
			var rect = new SDL.SDL_Rect { x = x - Camera.X, y = y - Camera.Y, w = w, h = h };
			SDL.SDL_RenderDrawRect(_renderer, ref rect);
		}

		protected void DrawRect(int x, int y, int w, int h, int palette, int color)
		{
			SetColor(palette, color);
			DrawRect(x, y, w, h);
		}

		protected void DrawSprite(int sprite, int x, int y, bool flipH = false, bool flipV = false)
		{
			var s = _sprites[sprite];
			var destRect = new SDL.SDL_Rect { x = x - Camera.X, y = y - Camera.Y, w = s.BufferRect.w, h = s.BufferRect.h };
			var flip = (flipH ? SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL : SDL.SDL_RendererFlip.SDL_FLIP_NONE) |
				(flipV ? SDL.SDL_RendererFlip.SDL_FLIP_VERTICAL : SDL.SDL_RendererFlip.SDL_FLIP_NONE);
			SDL.SDL_RenderCopyEx(_renderer, _spriteBuffer, ref s.BufferRect, ref destRect, 0, IntPtr.Zero, flip);
		}

		protected void DrawText(string text, int x, int y)
		{
			Text.Draw(_renderer, _spriteBuffer, text, x - Camera.X, y - Camera.Y, _drawColor);
		}

		protected void DrawText(string text, int x, int y, int palette, int color)
		{
			SetColor(palette, color);
			DrawText(text, x, y);
		}

		protected void FillRect(int x, int y, int w, int h)
		{
			var rect = new SDL.SDL_Rect { x = x - Camera.X, y = y - Camera.Y, w = w, h = h };
			SDL.SDL_RenderFillRect(_renderer, ref rect);
		}

		protected void FillRect(int x, int y, int w, int h, int palette, int color)
		{
			SetColor(palette, color);
			FillRect(x, y, w, h);
		}

		protected int MeasureText(string text) => Text.Measure(text);

		protected void SetClipRect(int x, int y, int w, int h)
		{
			var rect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h };
			SDL.SDL_RenderSetClipRect(_renderer, ref rect);
		}

		protected void SetColor(int palette, int color)
		{
			_drawColor = _palettes[palette][color];
			SDL.SDL_SetRenderDrawColor(_renderer, _drawColor.R, _drawColor.G, _drawColor.B, _drawColor.A);
		}

		protected void SetEffects(BlendMode mode)
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

		private static string SplitToLineLength(string val, int length)
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
