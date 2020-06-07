namespace Sharp16.Samples
{
	class Basics : SharpGame
	{
		public override void DrawBase()
		{
			DrawText("!\"#$%&'()*+,-./0123456789:;<=>?@", 2, 2, 0, 7);
			DrawText("ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`", 2, 18, 0, 8);
			DrawText("abcdefghijklmnopqrstuvwxyz{|}~", 2, 34, 0, 12);
			DrawSprite(0, 2, 64);
		}
	}
}

/* __Data__
__Palettes__
Y2RgYAiVZGQQZRGtnBzVu2r3f8NjjH+Ff/Cye/5f3lyvm/1v9W8A
__Sprites__
abc
def
__Maps__
789
024
*/
