using System;
using System.Collections.Generic;
using System.Text;
using Track.Relation.Tracks;

namespace Track.Relation.Transact
{
	public abstract class ValueObserverTransact<T> : ObjectTransact
	{
		public ValueObserverTransact()
			: this(null)
		{

		}
		public ValueObserverTransact(IEqualityComparer<T> comparer)
		{
			Track = new Track<T>(comparer);
		}

		private Track<T> Track { get; }

		protected abstract T GetValue();
		protected abstract void SetValue(T value);

		protected override void CommitData()
		{
			Track.SetValue(GetValue(), DispatcherTrack.Transaction);
		}
		protected override void RevertData()
		{
			if (Track.TryGetLastValue(out var result))
			{
				SetValue(result);
			}
		}
		protected override void OffsetData(int key)
		{
			if (Track.TryGetValue(key, out var result))
			{
				SetValue(result);
			}
		}
	}
}
