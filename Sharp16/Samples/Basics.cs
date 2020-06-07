namespace Sharp16.Samples
{
	class Basics : SharpGame
	{
		public override void DrawBase()
		{
			DrawText("!\"#$%&'()*+,-./0123456789:;<=>?@", 2, 2, 0, 7);
			DrawText("ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`", 2, 18, 0, 8);
			DrawText("abcdefghijklmnopqrstuvwxyz{|}~", 2, 34, 0, 12);
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 16; y++)
				{
					DrawSprite(0, x * 8 + 2, y * 8 + 64);
				}
			}

			DrawSprite(0, 160, 100, false, false);
			DrawSprite(0, 168, 100, true, false);
			DrawSprite(0, 160, 108, false, true);
			DrawSprite(0, 168, 108, true, true);
		}
	}
}

/* __Data__
__Palettes__
Y2RgYAiVZGQQZRGtnBzVu2r3f8NjjH+Ff/Cye/5f3lyvm/1v9W8A
__Sprites__
Y2QAAw4lQQYGIDZOC3UB4fJVMztAePe7u2dA+D9QzX8A
__Maps__
789
024
*/
