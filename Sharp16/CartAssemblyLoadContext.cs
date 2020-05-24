using System.Reflection;
using System.Runtime.Loader;

namespace Sharp16
{
	class CartAssemblyLoadContext : AssemblyLoadContext
	{
		public CartAssemblyLoadContext() : base(true) { }
		protected override Assembly Load(AssemblyName assemblyName) => null;
	}
}
