using SDL2;
using System;

namespace Sharp16
{
	public class Program
	{
		private const int SCREEN_WIDTH = 512;
		private const int SCREEN_HEIGHT = 288;

		private static IntPtr _window;
		private static IntPtr _screenSurface;
		private static IntPtr _renderer;
		private static Graphics _baseGraphics;
		private static Graphics _effectGraphics;
		private static Cartridge _cart;

		static void Main(string[] args)
		{
			Test();

			InitSDL();

			SDL.SDL_SetRenderDrawColor(_renderer, 0, 0xff, 0x8f, 0xff);

			bool run = true;
			while (run)
			{
				while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
				{
					switch (e.type)
					{
						case SDL.SDL_EventType.SDL_KEYDOWN:
							switch (e.key.keysym.sym)
							{
								case SDL.SDL_Keycode.SDLK_ESCAPE:
									run = false;
									break;
							}
							break;
						case SDL.SDL_EventType.SDL_QUIT:
							run = false;
							break;
					}
				}

				_cart.Game.Update();

				Draw();
			}

			CleanupSDL();
		}

		private static void Test()
		{
			_cart = new Cartridge(@"
using Sharp16;

namespace Test
{
	class TestGame : SharpGame
	{
		public override void DrawBase(Graphics g)
		{
			g.Clear(0, 0);
			for (int i = 0; i < 3; i++)
			{
				g.FillRect(10, 10 + i * 64, 64, 64, 0, i * 4 + 1);
				g.FillRect(74, 10 + i * 64, 64, 64, 0, i * 4 + 2);
				g.FillRect(138, 10 + i * 64, 64, 64, 0, i * 4 + 3);
				g.FillRect(202, 10 + i * 64, 64, 64, 0, i * 4 + 4);
			}
		}
	}
}
");
			_cart.Palettes.Add(new Color[16]);
			_cart.Palettes[0][0] = new Color(0, 0, 0, 1);
			_cart.Palettes[0][1] = new Color(7, 0, 0, 1);
			_cart.Palettes[0][2] = new Color(0, 7, 0, 1);
			_cart.Palettes[0][3] = new Color(0, 0, 7, 1);
			_cart.Palettes[0][4] = new Color(7, 7, 7, 1);
			_cart.Palettes[0][5] = new Color(15, 0, 0, 1);
			_cart.Palettes[0][6] = new Color(0, 15, 0, 1);
			_cart.Palettes[0][7] = new Color(0, 0, 15, 1);
			_cart.Palettes[0][8] = new Color(15, 15, 15, 1);
			_cart.Palettes[0][9] = new Color(31, 0, 0, 1);
			_cart.Palettes[0][10] = new Color(0, 31, 0, 1);
			_cart.Palettes[0][11] = new Color(0, 0, 31, 1);
			_cart.Palettes[0][12] = new Color(31, 31, 31, 1);
		}

		private static void InitSDL()
		{
			SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
			if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
			{
				Console.WriteLine("SDL init error: " + SDL.SDL_GetError());
			}

			_window = SDL.SDL_CreateWindow("Sharp 16", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
				SCREEN_WIDTH, SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

			if (_window == IntPtr.Zero)
			{
				Console.WriteLine("Window creation error: " + SDL.SDL_GetError());
			}

			_screenSurface = SDL.SDL_GetWindowSurface(_window);
			_renderer = SDL.SDL_CreateRenderer(_window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

			_baseGraphics = new Graphics(_renderer, _cart);
			_effectGraphics = new Graphics(_renderer, _cart);
		}

		private static void Draw()
		{
			SDL.SDL_RenderClear(_renderer);
			_cart.Game.DrawEffects(_effectGraphics);
			_cart.Game.DrawBase(_baseGraphics);
			// TODO - blend effects
			_cart.Game.DrawTop(_baseGraphics);
			SDL.SDL_RenderPresent(_renderer);
		}

		private static void CleanupSDL()
		{
			SDL.SDL_FreeSurface(_screenSurface);
			SDL.SDL_DestroyRenderer(_renderer);
			SDL.SDL_DestroyWindow(_window);
			SDL.SDL_Quit();
		}
	}
}
