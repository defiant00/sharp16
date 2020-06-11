﻿using SDL2;
using Sharp16.Firmware;
using System;

namespace Sharp16
{
	public class Sharp16
	{
		public const int SCREEN_WIDTH = 16 * 24;   // 384
		public const int SCREEN_HEIGHT = 9 * 24;   // 216
		public const int PLAYER_COUNT = 8;
		public const int SPRITE_BUFFER_SIZE = 2048;
		public const int FONT_SIZE = 128;

		private const int SCREEN_FPS = 60;
		private const int SCREEN_TICKS_PER_FRAME = 1000 / SCREEN_FPS;

		internal static Inputs[] _input = new Inputs[PLAYER_COUNT];

		private Config _config;
		private IntPtr _window;
		private IntPtr _renderer;
		private IntPtr _effectsBuffer;
		private IntPtr _font;
		private bool _fullscreen = false;
		private SharpOS _os;
		private int[,] _keyMap;

		internal Sharp16(string[] args)
		{
			_config = new Config(args);
		}

		internal void Run()
		{
			InitSDL();

			_os = new SharpOS(_config.Files.Count > 0 ? _config.Files[0] : null, _renderer, _effectsBuffer, _font);

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
								case SDL.SDL_Keycode.SDLK_s:
									if ((e.key.keysym.mod & SDL.SDL_Keymod.KMOD_CTRL) > 0)
									{
										_os.SaveCart();
										Console.WriteLine("Saved!");
									}
									break;
								case SDL.SDL_Keycode.SDLK_ESCAPE:
									_os.ToggleEditor();
									break;
							}
							break;
						case SDL.SDL_EventType.SDL_QUIT:
							run = false;
							break;
					}
				}

				// Update input state
				unsafe
				{
					byte* keys = (byte*)SDL.SDL_GetKeyboardState(out _);
					for (int i = 0; i < PLAYER_COUNT; i++)
					{
						_input[i].Prior = _input[i].Current;
						_input[i].Current.Up = (_keyMap[i, 0] > -1) && (keys[_keyMap[i, 0]] == 1);
						_input[i].Current.Down = (_keyMap[i, 1] > -1) && (keys[_keyMap[i, 1]] == 1);
						_input[i].Current.Left = (_keyMap[i, 2] > -1) && (keys[_keyMap[i, 2]] == 1);
						_input[i].Current.Right = (_keyMap[i, 3] > -1) && (keys[_keyMap[i, 3]] == 1);
						_input[i].Current.A = (_keyMap[i, 4] > -1) && (keys[_keyMap[i, 4]] == 1);
						_input[i].Current.B = (_keyMap[i, 5] > -1) && (keys[_keyMap[i, 5]] == 1);
						_input[i].Current.X = (_keyMap[i, 6] > -1) && (keys[_keyMap[i, 6]] == 1);
						_input[i].Current.Y = (_keyMap[i, 7] > -1) && (keys[_keyMap[i, 7]] == 1);
						_input[i].Current.L = (_keyMap[i, 8] > -1) && (keys[_keyMap[i, 8]] == 1);
						_input[i].Current.R = (_keyMap[i, 9] > -1) && (keys[_keyMap[i, 9]] == 1);
						_input[i].Current.Start = (_keyMap[i, 10] > -1) && (keys[_keyMap[i, 10]] == 1);
					}
				}

				// Update and draw
				_os.Update();
				_os.Draw();

				// Operate at a fixed 60 fps
				uint ticksTaken = SDL.SDL_GetTicks() - startTicks;
				if (ticksTaken < SCREEN_TICKS_PER_FRAME)
				{
					SDL.SDL_Delay(SCREEN_TICKS_PER_FRAME - ticksTaken);
				}
			}

			CleanupSDL();
		}

		private void InitSDL()
		{
			SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
			if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
			{
				Console.WriteLine("SDL init error: " + SDL.SDL_GetError());
			}

			// Video

			_window = SDL.SDL_CreateWindow("Sharp 16", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
				SCREEN_WIDTH, SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

			if (_window == IntPtr.Zero)
			{
				Console.WriteLine("Window creation error: " + SDL.SDL_GetError());
			}

			_renderer = SDL.SDL_CreateRenderer(_window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
			SDL.SDL_RenderSetLogicalSize(_renderer, SCREEN_WIDTH, SCREEN_HEIGHT);
			_effectsBuffer = SDL.SDL_CreateTexture(_renderer, SDL.SDL_PIXELFORMAT_ARGB8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, SCREEN_WIDTH, SCREEN_HEIGHT);
			SDL.SDL_SetTextureBlendMode(_effectsBuffer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

			_font = SDL.SDL_LoadBMP("font.bmp");
			if (_font == IntPtr.Zero)
			{
				Console.WriteLine("Unable to load font: " + SDL.SDL_GetError());
			}

			// Input

			_keyMap = new int[PLAYER_COUNT, 11];
			for (int i = 0; i < PLAYER_COUNT; i++)
			{
				for (int j = 0; j < 11; j++)
				{
					_keyMap[i, j] = -1;
				}
			}

			// TODO - save and load these
			_keyMap[0, 0] = (int)SDL.SDL_Scancode.SDL_SCANCODE_UP;
			_keyMap[0, 1] = (int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN;
			_keyMap[0, 2] = (int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT;
			_keyMap[0, 3] = (int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT;
			_keyMap[0, 4] = (int)SDL.SDL_Scancode.SDL_SCANCODE_Z;
			_keyMap[0, 5] = (int)SDL.SDL_Scancode.SDL_SCANCODE_X;
			_keyMap[0, 6] = (int)SDL.SDL_Scancode.SDL_SCANCODE_A;
			_keyMap[0, 7] = (int)SDL.SDL_Scancode.SDL_SCANCODE_S;
			_keyMap[0, 8] = (int)SDL.SDL_Scancode.SDL_SCANCODE_Q;
			_keyMap[0, 9] = (int)SDL.SDL_Scancode.SDL_SCANCODE_W;
			_keyMap[0, 10] = (int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN;
		}

		private void CleanupSDL()
		{
			_os.Unload();
			SDL.SDL_FreeSurface(_font);
			SDL.SDL_DestroyTexture(_effectsBuffer);
			SDL.SDL_DestroyRenderer(_renderer);
			SDL.SDL_DestroyWindow(_window);
			SDL.SDL_Quit();
		}
	}
}
