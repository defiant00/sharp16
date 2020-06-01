using SDL2;
using System;

namespace Sharp16
{
	public class Program
	{
		private const int SCREEN_WIDTH = 16 * 32;	// 512
		private const int SCREEN_HEIGHT = 9 * 32;	// 288
		private const int SCREEN_FPS = 60;
		private const int SCREEN_TICKS_PER_FRAME = 1000 / SCREEN_FPS;

		private static IntPtr _window;
		private static IntPtr _renderer;
		private static IntPtr _effectsBuffer;
		private static bool _fullscreen = false;
		private static Cartridge _cart;

		private static int _keyUp = (int)SDL.SDL_Scancode.SDL_SCANCODE_UP;
		private static int _keyDown = (int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN;
		private static int _keyLeft = (int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT;
		private static int _keyRight = (int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT;
		private static int _keyA = (int)SDL.SDL_Scancode.SDL_SCANCODE_Z;
		private static int _keyB = (int)SDL.SDL_Scancode.SDL_SCANCODE_X;
		private static int _keyX = (int)SDL.SDL_Scancode.SDL_SCANCODE_A;
		private static int _keyY = (int)SDL.SDL_Scancode.SDL_SCANCODE_S;
		private static int _keyL = (int)SDL.SDL_Scancode.SDL_SCANCODE_Q;
		private static int _keyR = (int)SDL.SDL_Scancode.SDL_SCANCODE_W;
		private static int _keyStart = (int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN;


		static void Main(string[] args)
		{
			InitSDL();

			Test();

			bool run = true;
			while (run)
			{
				uint startTicks = SDL.SDL_GetTicks();

				while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
				{
					switch (e.type)
					{
						case SDL.SDL_EventType.SDL_KEYDOWN:
							switch (e.key.keysym.sym)
							{
								case SDL.SDL_Keycode.SDLK_RETURN:
									if ((e.key.keysym.mod & SDL.SDL_Keymod.KMOD_ALT) > 0)
									{
										_fullscreen = !_fullscreen;
										SDL.SDL_SetWindowFullscreen(_window, _fullscreen ? (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0);
									}
									break;
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

				// Update input state
				_cart.Game.Input.Prior = _cart.Game.Input.Current;
				unsafe
				{
					byte* keys = (byte*)SDL.SDL_GetKeyboardState(out _);
					_cart.Game.Input.Current.Up = keys[_keyUp] == 1;
					_cart.Game.Input.Current.Down = keys[_keyDown] == 1;
					_cart.Game.Input.Current.Left = keys[_keyLeft] == 1;
					_cart.Game.Input.Current.Right = keys[_keyRight] == 1;
					_cart.Game.Input.Current.A = keys[_keyA] == 1;
					_cart.Game.Input.Current.B = keys[_keyB] == 1;
					_cart.Game.Input.Current.X = keys[_keyX] == 1;
					_cart.Game.Input.Current.Y = keys[_keyY] == 1;
					_cart.Game.Input.Current.L = keys[_keyL] == 1;
					_cart.Game.Input.Current.R = keys[_keyR] == 1;
					_cart.Game.Input.Current.Start = keys[_keyStart] == 1;
				}

				// Update and draw game
				_cart.Game.Update();
				_cart.Game.Draw();

				// Operate at a fixed 60 fps
				uint ticksTaken = SDL.SDL_GetTicks() - startTicks;
				if (ticksTaken < SCREEN_TICKS_PER_FRAME)
				{
					SDL.SDL_Delay(SCREEN_TICKS_PER_FRAME - ticksTaken);
				}
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
		public int x = 30, y = 30;

		public override void Update()
		{
			if (Input.Current.Up) y--;
			else if (Input.Current.Down) y++;

			if (Input.Current.Left) x--;
			else if (Input.Current.Right) x++;

			if (Input.Current.B && !Input.Prior.B)
			{
				x = 30;
				y = 30;
			}
		}

		public override void DrawBase()
		{
			SetClipRect(50, 50, 100, 100);
			Clear(0, 0);
			for (int i = 0; i < 3; i++)
			{
				FillRect(10, 10 + i * 64, 64, 64, 0, i * 4 + 1);
				FillRect(74, 10 + i * 64, 64, 64, 0, i * 4 + 2);
				FillRect(138, 10 + i * 64, 64, 64, 0, i * 4 + 3);
				FillRect(202, 10 + i * 64, 64, 64, 0, i * 4 + 4);
			}
		}

		public override void DrawEffects()
		{
			SetEffects(BlendMode.Add);
			FillRect(x, y, 128, 128, 1, 0);
		}
	}
}
");
			_cart.Game._palettes.Add(new Color[16]);
			_cart.Game._palettes[0][0] = new Color(4, 4, 4, 1);
			_cart.Game._palettes[0][1] = new Color(7, 0, 0, 1);
			_cart.Game._palettes[0][2] = new Color(0, 7, 0, 1);
			_cart.Game._palettes[0][3] = new Color(0, 0, 7, 1);
			_cart.Game._palettes[0][4] = new Color(7, 7, 7, 1);
			_cart.Game._palettes[0][5] = new Color(15, 0, 0, 1);
			_cart.Game._palettes[0][6] = new Color(0, 15, 0, 1);
			_cart.Game._palettes[0][7] = new Color(0, 0, 15, 1);
			_cart.Game._palettes[0][8] = new Color(15, 15, 15, 1);
			_cart.Game._palettes[0][9] = new Color(31, 0, 0, 1);
			_cart.Game._palettes[0][10] = new Color(0, 31, 0, 1);
			_cart.Game._palettes[0][11] = new Color(0, 0, 31, 1);
			_cart.Game._palettes[0][12] = new Color(31, 31, 31, 1);

			_cart.Game._palettes.Add(new Color[16]);
			_cart.Game._palettes[1][0] = new Color(15, 15, 0, 1);

			_cart.Game._renderer = _renderer;
			_cart.Game._effectsBuffer = _effectsBuffer;
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

			_renderer = SDL.SDL_CreateRenderer(_window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
			SDL.SDL_RenderSetLogicalSize(_renderer, SCREEN_WIDTH, SCREEN_HEIGHT);
			_effectsBuffer = SDL.SDL_CreateTexture(_renderer, SDL.SDL_PIXELFORMAT_RGBA8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, SCREEN_WIDTH, SCREEN_HEIGHT);
			SDL.SDL_SetTextureBlendMode(_effectsBuffer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
		}

		private static void CleanupSDL()
		{
			SDL.SDL_DestroyTexture(_effectsBuffer);
			SDL.SDL_DestroyRenderer(_renderer);
			SDL.SDL_DestroyWindow(_window);
			SDL.SDL_Quit();
		}
	}
}
