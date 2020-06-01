namespace Sharp16.Firmware
{
	public class MainMenu : SharpGame
	{
		public MainMenu()
		{
			// PICO-8 palette
			_palettes.Add(new[]
			{
				new Color(0, 0, 0, 1),		// black
				new Color(3, 5, 10, 1),		// dark blue
				new Color(15, 4, 10, 1),	// dark purple
				new Color(0, 16, 10, 1),	// dark green
				new Color(21, 10, 6, 1),	// brown
				new Color(11, 10, 9, 1),	// dark grey
				new Color(24, 24, 24, 1),	// light grey
				new Color(31, 30, 29, 1),	// white
				new Color(31, 0, 9, 1),		// red
				new Color(31, 20, 0, 1),	// orange
				new Color(31, 29, 4, 1),	// yellow
				new Color(0, 28, 6, 1),		// green
				new Color(5, 21, 31, 1),	// blue
				new Color(16, 14, 19, 1),	// lavender
				new Color(31, 14, 21, 1),	// pink
				new Color(31, 25, 21, 1),	// light peach
			});
		}

		public override void DrawBase()
		{
			Clear(0, 12);
			for (int i = 0; i < 16; i++)
				FillRect(i * 24 +1, 100, 22, 22, 0, i);
		}
	}
}
