using System;
using System.Collections.Generic;
using System.Text;
using Track.Relation.Tracks;

namespace Track.Relation.Transact
{
	public class ValueTransact<T> : ObjectTransact
	{
		public ValueTransact()
		{

		}
		public ValueTransact(DispatcherTrack dispatcherTrack)
			: base(dispatcherTrack)
		{

		}
		public ValueTransact(T value)
		{
			Value = value;
		}
		public ValueTransact(T value, DispatcherTrack dispatcherTrack)
			: base(dispatcherTrack)
		{
			Value = value;
		}

		public IEqualityComparer<T> Comparer => Track.Comparer;
		private Track<T> Track { get; } = new Track<T>();

		public T Value
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
		private T value;

		public bool IsUndefined { get; private set; } = true;
		public void Close()
		{
			IsUndefined = true;
		}

		protected override void OffsetData(int key)
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
