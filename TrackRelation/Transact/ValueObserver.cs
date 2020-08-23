using System;
using System.Collections.Generic;
using System.Text;
using Track.Relation.Tracks;

namespace Track.Relation.Transact
{
	public class ValueObserver<T> : ObjectTransact
	{
		public ValueObserver(IValueAccess<T> valueAccess = null, IEqualityComparer<T> comparer = null, DispatcherTrack dispatcher = null)
			: base(dispatcher)
		{
			ValueAccess = valueAccess;
			Track = new Track<T>(comparer);
		}

		public IValueAccess<T> ValueAccess { get; set; }

		public IEqualityComparer<T> Comparer => Track.Comparer;
		private Track<T> Track { get; }

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
		protected override void OffsetData(int key)
		{
			if (Track.TryGetValue(key, out var result))
			{
				ValueAccess.SetValue(result);
			}
		}
	}
}
