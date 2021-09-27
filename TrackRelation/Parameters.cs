using System.Collections.Generic;

namespace Track.Relation
{
	public class Parameters<TKey, TValue> : IParameters<TKey, TValue>
	{
		public IComparer<TKey> KeyComparer { get; set; } = Comparer<TKey>.Default;
		public IEqualityComparer<TValue> ValueComparer { get; set; } = EqualityComparer<TValue>.Default;
		public ICloner<TValue> ValueCloner { get; set; } = Cloner<TValue>.Instance;
	}
}
