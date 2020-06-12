using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SDL2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Sharp16
{
	internal class CartAssemblyLoadContext : AssemblyLoadContext
	{
		internal CartAssemblyLoadContext() : base(true) { }
		protected override Assembly Load(AssemblyName assemblyName) => null;
	}

	public class Sharp16
	{
		public const int SCREEN_WIDTH = 16 * 24;   // 384
		public const int SCREEN_HEIGHT = 9 * 24;   // 216
		public const int PLAYER_COUNT = 8;
		public const int SPRITE_BUFFER_SIZE = 2048;
		public const int FONT_SIZE = 128;

		private const int SCREEN_FPS = 60;
		private const int SCREEN_TICKS_PER_FRAME = 1000 / SCREEN_FPS;

		private const string DATA_START = "/* __Data__";
		private const string DATA_END = "*/";
		private const string MAPS_START = "__Maps__";
		private const string PALETTES_START = "__Palettes__";
		private const string SPRITES_START = "__Sprites__";

		private static readonly string SystemRuntimeLocation = Assembly.Load("System.Runtime").Location;

		internal static Inputs[] _input = new Inputs[PLAYER_COUNT];

		private Config _config;
		private IntPtr _window;
		private IntPtr _renderer;
		private IntPtr _effectsBuffer;
		private IntPtr _fontSurface;
		private IntPtr _fontTexture;
		private bool _fullscreen = false;
		private int[,] _keyMap;
		private SharpGame _game = new SharpGame();
		private CartAssemblyLoadContext _cartAssemblyContext;
		private bool _inEditor;

		private string _cartFileName;
		private string _cartCode;
		private string _cartPalettes;
		private string _cartSprites;
		private string _cartMaps;

		internal Sharp16(string[] args)
		{
			_config = new Config(args);
		}

		internal void Run()
		{
			Init();

			if (_config.Files.Count > 0)
			{
				_cartFileName = _config.Files[0];
				ParseCart(File.ReadAllText(_cartFileName));
				CompileCart();
				_game._renderer = _renderer;
				_game._effectsBuffer = _effectsBuffer;
				_game._fontSurface = _fontSurface;
				LoadCartData();
				_game.BuildSpriteBuffer();
			}

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
										SaveCart();
										Console.WriteLine("Saved!");
									}
									break;
								case SDL.SDL_Keycode.SDLK_ESCAPE:
									_inEditor = !_inEditor;
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

				Update();
				Draw();

				// Operate at a fixed 60 fps
				uint ticksTaken = SDL.SDL_GetTicks() - startTicks;
				if (ticksTaken < SCREEN_TICKS_PER_FRAME)
				{
					SDL.SDL_Delay(SCREEN_TICKS_PER_FRAME - ticksTaken);
				}
			}

			Cleanup();
		}

		private void Draw()
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
			else { _game.Draw(); }
		}

		private void Update()
		{
			if (_inEditor) { }
			else { _game.Update(); }
		}

		private void ParseCart(string input)
		{
			int ind = input.IndexOf(DATA_START);
			if (ind > -1)
			{
				_cartCode = input.Substring(0, ind);
				string data = input.Substring(ind);
				string[] lines = data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				string section = null;
				var dataBuffer = new StringBuilder();
				foreach (string line in lines)
				{
					string tLine = line.Trim();
					if (tLine.StartsWith("__"))
					{
						PopulateCartData(section, dataBuffer.ToString());
						section = tLine;
						dataBuffer.Clear();
					}
					else if (tLine != DATA_END)
					{
						dataBuffer.Append(tLine);
					}
				}
				PopulateCartData(section, dataBuffer.ToString());
			}
			else { _cartCode = input; }
		}

		private void PopulateCartData(string section, string data)
		{
			switch (section)
			{
				case PALETTES_START:
					_cartPalettes = data;
					break;
				case SPRITES_START:
					_cartSprites = data;
					break;
				case MAPS_START:
					_cartMaps = data;
					break;
			}
		}

		private void CompileCart()
		{
			Console.Write("Compiling...");

			var compilation = CSharpCompilation.Create("cart.dll", new[] { SyntaxFactory.ParseSyntaxTree(_cartCode) },
				new[]
				{
					MetadataReference.CreateFromFile(typeof(Program).Assembly.Location),
					MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
					MetadataReference.CreateFromFile(SystemRuntimeLocation),
					MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
				},
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			var errors = compilation.GetDiagnostics();
			if (errors.Length > 0)
			{
				Console.WriteLine("Errors:");
				foreach (var e in errors)
				{
					Console.WriteLine(e);
				}
			}
			else
			{
				_cartAssemblyContext?.Unload();
				_cartAssemblyContext = new CartAssemblyLoadContext();
				Assembly compiledAssembly;
				using (var ms = new MemoryStream())
				{
					var res = compilation.Emit(ms);
					ms.Seek(0, SeekOrigin.Begin);
					compiledAssembly = _cartAssemblyContext.LoadFromStream(ms);
				}

				var compiledType = compiledAssembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(SharpGame)));
				if (compiledType == null)
				{
					Console.WriteLine("Error:");
					Console.WriteLine("No SharpGame defined.");
				}
				else
				{
					_game = (SharpGame)Activator.CreateInstance(compiledType);
					Console.WriteLine("Done!");
				}
			}
		}

		private void SaveCart()
		{
			var lines = new List<string> { _cartCode, DATA_START };
			string palettes = _game.CompressedPalettes;
			if (!string.IsNullOrEmpty(palettes))
			{
				lines.Add(PALETTES_START);
				lines.Add(palettes);
			}
			string sprites = _game.CompressedSprites;
			if (!string.IsNullOrEmpty(sprites))
			{
				lines.Add(SPRITES_START);
				lines.Add(sprites);
			}
			string maps = _game.CompressedMaps;
			if (!string.IsNullOrEmpty(maps))
			{
				lines.Add(MAPS_START);
				lines.Add(maps);
			}
			lines.Add(DATA_END);
			File.WriteAllLines(_cartFileName, lines);
		}

		private void LoadCartData()
		{
			_game.CompressedPalettes = _cartPalettes;
			_cartPalettes = null;

			_game.CompressedSprites = _cartSprites;
			_cartSprites = null;

			// TODO - maps
			_cartMaps = null;
		}

		private void Init()
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

			_fontSurface = SDL.SDL_LoadBMP("font.bmp");
			if (_fontSurface == IntPtr.Zero)
			{
				Console.WriteLine("Unable to load font: " + SDL.SDL_GetError());
			}
			_fontTexture = SDL.SDL_CreateTextureFromSurface(_renderer, _fontSurface);

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

		private void Cleanup()
		{
			_game?.Unload();
			_game = null;
			_cartAssemblyContext?.Unload();
			SDL.SDL_DestroyTexture(_fontTexture);
			SDL.SDL_FreeSurface(_fontSurface);
			SDL.SDL_DestroyTexture(_effectsBuffer);
			SDL.SDL_DestroyRenderer(_renderer);
			SDL.SDL_DestroyWindow(_window);
			SDL.SDL_Quit();
		}
	}
}
