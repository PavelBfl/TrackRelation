using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Transact
{
	interface IIndicesCollection<TKey, TValue>
	{
		bool TryGetValue(TKey key, out TValue value);
		void Add(TKey key, TValue valueTrack);
	}
}
