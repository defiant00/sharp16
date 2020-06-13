namespace Sharp16
{
	internal struct Mouse
	{
		internal Vector2 Position;
		internal MouseButtons Current, Prior;
	}

	internal struct MouseButtons
	{
		internal bool Left, Right;
	}

	public struct Inputs
	{
		public InputState Current, Prior;
	}

	public struct InputState
	{
		public bool Up, Down, Left, Right, A, B, X, Y, L, R, Start;
	}
}
