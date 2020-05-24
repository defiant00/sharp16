using System.Collections.Generic;

namespace Sharp16
{
	class Cartridge
	{
		public List<Color[]> Palettes = new List<Color[]>();
		public SharpGame Game = new SharpGame();	// Should probably remove this later to not create an extra object that's immediately replaced

		public Cartridge(string code)
		{

		}
	}
}
