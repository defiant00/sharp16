namespace Sharp16.Samples
{
	class Basics : SharpGame
	{
		public override void DrawBase()
		{
			Camera = new Vector2(-1, -1);
			DrawText("A A\tA", 0, 0, 0, 7);
			DrawText("!\"#$%&'()*+,-./", 0, 16, 0, 8);
			DrawText("0123456789:;<=>?@", 0, 32, 0, 11);
			DrawText("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 0, 48, 0, 12);
			DrawText("[\\]^_`", 0, 64, 0, 7);
			DrawText("abcdefghijklmnopqrstuvwxyz", 0, 80, 0, 8);
			DrawText("{|}~", 0, 96, 0, 11);
			DrawText("And then she said, \"...hey.\"", 0, 112, 0, 12);

			DrawText("\tThis is the start of a paragraph.", 0, 160, 0, 7);
			DrawText("And this is the second line.", 0, 176, 0, 7);
			DrawText("And one more, just to make it look proper!", 0, 192, 0, 7);
		}
	}
}

/* __Data__
__Palettes__
123
456
__Sprites__
abc
def
__Maps__
789
024
*/
