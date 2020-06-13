namespace Sharp16.Samples
{
	class Basics : SharpGame
	{
		public int xp, yp;

		public override void Init()
		{
			SetEffects(BlendMode.Add);
		}

		public override void Update()
		{
			if (Input[0].Current.Up) yp--;
			else if (Input[0].Current.Down) yp++;
			if (Input[0].Current.Left) xp--;
			else if (Input[0].Current.Right) xp++;
		}

		public override void DrawBase()
		{
			Clear(0, 1);
			DrawText("!\"#$%&'()*+,-./0123456789:;<=>?@", 2, 18, 0, 7);
			DrawText("ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`", 2, 34, 0, 8);
			DrawText("abcdefghijklmnopqrstuvwxyz{|}~", 2, 50, 0, 12);
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 16; y++)
				{
					DrawSprite(0, x * 8 + 2, y * 8 + 80);
				}
			}
		}

		public override void DrawEffects()
		{
			DrawSprite(0, 160 + xp, 100 + yp, false, false);
			DrawSprite(0, 168 + xp, 100 + yp, true, false);
			DrawSprite(0, 160 + xp, 108 + yp, false, true);
			DrawSprite(0, 168 + xp, 108 + yp, true, true);
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
