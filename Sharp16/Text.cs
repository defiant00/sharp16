using SDL2;
using System;
using System.Collections.Generic;

namespace Sharp16
{
	internal static class Text
	{
		private static readonly Dictionary<char, SDL.SDL_Rect> _glyphs = new Dictionary<char, SDL.SDL_Rect>
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

		internal static void Draw(IntPtr renderer, IntPtr font, string text, int x, int y, Color color)
		{
			SDL.SDL_SetTextureColorMod(font, color.R, color.G, color.B);
			SDL.SDL_SetTextureAlphaMod(font, color.A);

			int curX = x;
			foreach (char c in text)
			{
				if (_glyphs.ContainsKey(c))
				{
					var g = _glyphs[c];
					if (g.h > 0)
					{
						var destRect = new SDL.SDL_Rect { x = curX, y = y, w = g.w, h = g.h };
						SDL.SDL_RenderCopy(renderer, font, ref g, ref destRect);
					}
					curX += g.w + 1;
				}
			}

			SDL.SDL_SetTextureColorMod(font, 255, 255, 255);
			SDL.SDL_SetTextureAlphaMod(font, 255);
		}

		internal static int Measure(string text)
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
	}
}
