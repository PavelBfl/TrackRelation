using System;
using System.Collections.Generic;
using System.Text;
using Track.Relation.Tracks;

namespace Track.Relation.Transact
{
	public class ValueTransact<TKey, TValue> : ObjectTransact<TKey>
	{
		public ValueTransact(TValue value, IEqualityComparer<TValue> comparer = null, DispatcherTrack<TKey> dispatcher = null)
			: this(comparer, dispatcher)
		{
			Value = value;
		}
		public ValueTransact(IEqualityComparer<TValue> comparer = null, DispatcherTrack<TKey> dispatcher = null)
			: base(dispatcher)
		{
			Track = new Track<TKey, TValue>(comparer);
		}

		public IEqualityComparer<TValue> Comparer => Track.Comparer;
		private Track<TKey, TValue> Track { get; }

		public TValue Value
		{
			get
			{
				if (IsUndefined)
				{
					throw new InvalidOperationException();
				}
				return value;
			}
			set
			{
				ThrowIfCommitedEnable();
				this.value = value;
				IsUndefined = false;
			}
		}
		private TValue value;

		public bool IsUndefined { get; private set; } = true;
		public void Close()
		{
			IsUndefined = true;
		}

		protected override void OffsetData(TKey key)
		{
			if (Track.TryGetValue(key, out var value))
			{
				Value = value;
			}
			else
			{
				IsUndefined = true;
			}
		}

		protected override void RevertData()
		{
			if (Track.TryGetLastValue(out var value))
			{
				Value = value;
			}
			else
			{
				IsUndefined = true;
			}
		}

		protected override void CommitData()
		{
			if (IsUndefined)
			{
				Track.Close(DispatcherTrack.Transaction);
			}
			else
			{
				Track.SetValue(Value, DispatcherTrack.Transaction); 
			}
		}
	}
}
