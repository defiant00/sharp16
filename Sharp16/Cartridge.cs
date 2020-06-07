using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Sharp16
{
	public class CartAssemblyLoadContext : AssemblyLoadContext
	{
		public CartAssemblyLoadContext() : base(true) { }
		protected override Assembly Load(AssemblyName assemblyName) => null;
	}

	public class Cartridge
	{
		public SharpGame Game = new SharpGame();
		private static readonly string SystemRuntimeLocation = Assembly.Load("System.Runtime").Location;
		private CartAssemblyLoadContext _context;

		private const string DATA_START = "/* __Data__";
		private const string DATA_END = "*/";
		private const string MAPS_START = "__Maps__";
		private const string PALETTES_START = "__Palettes__";
		private const string SPRITES_START = "__Sprites__";

		private string _filename;
		private string _code;
		private string _maps;
		private string _palettes;
		private string _sprites;

		public Cartridge(SharpGame game, IntPtr renderer, IntPtr effectsBuffer, IntPtr font)
		{
			Game = game;
			Game._renderer = renderer;
			Game._effectsBuffer = effectsBuffer;
			Game._font = font;
		}

		public Cartridge(string filename, IntPtr renderer, IntPtr effectsBuffer, IntPtr font)
		{
			_filename = filename;
			Parse(File.ReadAllText(filename));
			Compile();
			Game._renderer = renderer;
			Game._effectsBuffer = effectsBuffer;
			Game._font = font;
			LoadData();
			Game.BuildSpriteBuffer();
		}

		public void Save()
		{
			var lines = new List<string> { _code, DATA_START };
			string palettes = Game._compressedPalettes;
			if (!string.IsNullOrEmpty(palettes))
			{
				lines.Add(PALETTES_START);
				lines.Add(palettes);
			}
			string sprites = Game._compressedSprites;
			if (!string.IsNullOrEmpty(sprites))
			{
				lines.Add(SPRITES_START);
				lines.Add(sprites);
			}
			string maps = Game._compressedMaps;
			if (!string.IsNullOrEmpty(maps))
			{
				lines.Add(MAPS_START);
				lines.Add(maps);
			}
			lines.Add(DATA_END);
			File.WriteAllLines(_filename, lines);
		}

		private void LoadData()
		{
			Game._compressedPalettes = _palettes;
			_palettes = null;
			// TODO - sprites

			Game._sprites.Add(new Sprite
			{
				Size = 8,
				Palette = 0,
				Data = new byte[64]{
					0,0,1,1,2,2,3,3,
					0,0,1,1,2,2,3,3,
					4,4,5,5,6,6,7,7,
					4,4,5,5,6,6,7,7,
					8,8,9,9,10,10,11,11,
					8,8,9,9,10,10,11,11,
					12,12,13,13,14,14,15,15,
					12,12,13,13,14,14,15,15,
				}
			});


			_sprites = null;
			// TODO - maps
			_maps = null;
		}

		private void Parse(string input)
		{
			int ind = input.IndexOf(DATA_START);
			if (ind > -1)
			{
				_code = input.Substring(0, ind);
				string data = input.Substring(ind);
				string[] lines = data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				string section = null;
				var dataBuffer = new StringBuilder();
				foreach (string line in lines)
				{
					string tLine = line.Trim();
					if (tLine.StartsWith("__"))
					{
						PopulateData(section, dataBuffer.ToString());
						section = tLine;
						dataBuffer.Clear();
					}
					else if (tLine != DATA_END)
					{
						dataBuffer.Append(tLine);
					}
				}
				PopulateData(section, dataBuffer.ToString());
			}
			else { _code = input; }
		}

		private void PopulateData(string section, string data)
		{
			switch (section)
			{
				case MAPS_START:
					_maps = data;
					break;
				case PALETTES_START:
					_palettes = data;
					break;
				case SPRITES_START:
					_sprites = data;
					break;
			}
		}

		private void Compile()
		{
			Console.Write("Compiling...");

			var compilation = CSharpCompilation.Create("cartridge.dll", new[] { SyntaxFactory.ParseSyntaxTree(_code) },
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
				_context = new CartAssemblyLoadContext();
				Assembly compiledAssembly;
				using (var ms = new MemoryStream())
				{
					var res = compilation.Emit(ms);
					ms.Seek(0, SeekOrigin.Begin);
					compiledAssembly = _context.LoadFromStream(ms);
				}

				var compiledType = compiledAssembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(SharpGame)));
				if (compiledType == null)
				{
					Console.WriteLine("Error:");
					Console.WriteLine("No SharpGame defined.");
				}
				else
				{
					Game = (SharpGame)Activator.CreateInstance(compiledType);
					Console.WriteLine("Done!");
				}
			}
		}

		public void Unload()
		{
			Game.Unload();
			Game = null;
			_context?.Unload();
		}
	}
}
