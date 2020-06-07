namespace Sharp16.Firmware
{
	public class MainMenu : SharpGame
	{
		public MainMenu()
		{
			// PICO-8 palette
			CompressedPalettes = "Y2RgYAiVZGQQZRGtnBzVu2r3f8NjjH+Ff/Cye/5f3lyvm/1v9W8A";
		}

		public override void DrawBase()
		{
			Clear(0, 12);
			for (int i = 0; i < 16; i++)
				FillRect(i * 24 +1, 100, 22, 22, 0, i);
		}
	}
}
