using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Transact
{
	public abstract class DataObserverTransact<T> : ObjectTransact
	{
		public DataObserverTransact()
			: this(null)
		{

		}
		public DataObserverTransact(IEqualityComparer<T> comparer)
		{
			Track = new ValueTrack<T>(comparer);
		}

		private ValueTrack<T> Track { get; }

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
