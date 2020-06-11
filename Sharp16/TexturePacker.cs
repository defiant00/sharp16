using System;

namespace Sharp16
{
	internal class TexturePacker
	{
		private TextureBlock _block = new TextureBlock(0, 0, Sharp16.SPRITE_BUFFER_SIZE);

		internal Vector2 Add(int size)
		{
			var res = _block.Add(size);
			if (res.Added)
			{
				return new Vector2(res.X, res.Y);
			}
			throw new Exception($"No space to add a texture of size {size}");
		}

		private struct TextureBlockAddResult
		{
			internal bool Added;
			internal int X, Y;

			internal TextureBlockAddResult(bool added, int x, int y)
			{
				Added = added;
				X = x;
				Y = y;
			}
		}

		private class TextureBlock
		{
			internal int X, Y, Size;
			private TextureBlock[,] _children;
			private bool _full;

			private bool Empty => !_full && (_children == null ||
				(
					_children[0, 0].Empty &&
					_children[0, 1].Empty &&
					_children[1, 0].Empty &&
					_children[1, 1].Empty
				));

			internal TextureBlock(int x, int y, int size)
			{
				X = x;
				Y = y;
				Size = size;
				if (size > Sprite.MIN_SIZE)
				{
					int div2 = size / 2;
					_children = new TextureBlock[2, 2];
					for (int yi = 0; yi < 2; yi++)
					{
						for (int xi = 0; xi < 2; xi++)
						{
							_children[xi, yi] = new TextureBlock(xi * div2 + x, yi * div2 + y, div2);
						}
					}
				}
			}

			internal TextureBlockAddResult Add(int size)
			{
				if (_full) { return new TextureBlockAddResult(false, 0, 0); }

				if (Size == size && Empty)
				{
					_full = true;
					return new TextureBlockAddResult(true, X, Y);
				}

				if (size < Size)
				{
					for (int yi = 0; yi < 2; yi++)
					{
						for (int xi = 0; xi < 2; xi++)
						{
							var res = _children[xi, yi].Add(size);
							if (res.Added)
							{
								_full = _children[0, 0]._full &&
									_children[0, 1]._full &&
									_children[1, 0]._full &&
									_children[1, 1]._full;
								return res;
							}
						}
					}
				}

				return new TextureBlockAddResult(false, 0, 0);
			}
		}
	}
}
