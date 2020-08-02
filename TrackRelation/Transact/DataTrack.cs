﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Transact
{
	public class DataTransact<T> : ObjectTransact
	{
		public DataTransact()
		{

		}
		public DataTransact(DispatcherTrack dispatcherTrack)
			: base(dispatcherTrack)
		{

		}
		public DataTransact(T value)
		{
			Value = value;
		}
		public DataTransact(T value, DispatcherTrack dispatcherTrack)
			: base(dispatcherTrack)
		{
			Value = value;
		}

		public IEqualityComparer<T> Comparer { get; }
		private ValueTrack<T> Track { get; } = new ValueTrack<T>();

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
				Track.Close(DispatcherTrack.KeyBatch);
			}
			else
			{
				Track.TrySetValue(Value, Comparer, DispatcherTrack.KeyBatch); 
			}
		}
	}
}
