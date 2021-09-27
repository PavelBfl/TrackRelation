namespace Track.Relation
{
	class Cloner<T> : ICloner<T>
	{
		public static Cloner<T> Instance { get; } = new Cloner<T>();
		private Cloner()
		{

		}
		public T Clone(T value) => value;
	}
}
