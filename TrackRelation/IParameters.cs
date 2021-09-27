using System.Collections.Generic;

namespace Track.Relation
{
	public interface IParameters<TKey, TValue>
	{
		IComparer<TKey> KeyComparer { get; }
		IEqualityComparer<TValue> ValueComparer { get; }
		ICloner<TValue> ValueCloner { get; }
	}
}
