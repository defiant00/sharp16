using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Reflection;

namespace Sharp16
{
	public class Cartridge
	{
		public SharpGame Game = new SharpGame();    // Should probably remove the default initialization later
		private static readonly string SystemRuntimeLocation = Assembly.Load("System.Runtime").Location;
		private CartAssemblyLoadContext _context;

		public Cartridge(string code)
		{
			Console.Write("Compiling...");

			var compilation = CSharpCompilation.Create("cartridge.dll", new[] { SyntaxFactory.ParseSyntaxTree(code) },
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
				var compiledType = compiledAssembly.GetType("Test.TestGame");       // TODO - check all types and run the first that inherits from SharpGame
				Game = (SharpGame)Activator.CreateInstance(compiledType);
				Console.WriteLine("Done!");
			}
		}

		~Cartridge()
		{
			_context.Unload();
		}
	}
}
