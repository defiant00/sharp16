namespace Sharp16
{
	public struct Inputs
	{
		public InputState Current, Prior;
	}

	public struct InputState
	{
		public bool Up, Down, Left, Right, A, B, X, Y, L, R, Start;
	}
}
