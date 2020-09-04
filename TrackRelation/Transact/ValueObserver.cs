using System;
using System.Collections.Generic;
using System.Text;
using Track.Relation.Tracks;

namespace Track.Relation.Transact
{
	public class ValueObserver<TKey, TValue> : ObjectTransact<TKey>
	{
		public ValueObserver(IValueAccess<TValue> valueAccess = null, IEqualityComparer<TValue> comparer = null, DispatcherTrack<TKey> dispatcher = null)
			: base(dispatcher)
		{
			ValueAccess = valueAccess;
			Track = new Track<TKey, TValue>(comparer);
		}

		public IValueAccess<TValue> ValueAccess { get; set; }

		public IEqualityComparer<TValue> Comparer => Track.Comparer;
		private Track<TKey, TValue> Track { get; }

		protected override void CommitData()
		{
			Track.SetValue(ValueAccess.GetValue(), DispatcherTrack.Transaction);
		}
		protected override void RevertData()
		{
			if (Track.TryGetLastValue(out var result))
			{
				ValueAccess.SetValue(result);
			}
		}
		protected override void OffsetData(TKey key)
		{
			if (Track.TryGetValue(key, out var result))
			{
				ValueAccess.SetValue(result);
			}
		}
	}
}
