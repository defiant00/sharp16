using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sharp16
{
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

		private string _code;
		private string _maps;
		private string _palettes;
		private string _sprites;

		public Cartridge() { }

		public Cartridge(string input)
		{
			Parse(input);
			Build();
		}

		public void Save(string file)
		{
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

		private void Build()
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

		~Cartridge()
		{
			_context?.Unload();
		}
	}
}
