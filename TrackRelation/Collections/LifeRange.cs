using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Collections
{
	struct LifeRange<T>
	{
		private LifeRange(int begin, int? end, T item)
		{
			Begin = begin;
			End = end;
			Item = item;
		}
		public LifeRange(int begin, int end, T item)
			: this(begin, new int?(end), item)
		{

		}
		public LifeRange(int begin, T item)
			: this(begin, null, item)
		{

		}

		public int Begin { get; }
		public int? End { get; }
		public T Item { get; }

		public bool Contains(int keyTrack)
		{
			return Begin <= keyTrack && keyTrack < (End ?? int.MaxValue);
		}

		public LifeRange<T> Close(int keyTrack)
		{
			return new LifeRange<T>(Begin, keyTrack, Item);
		}
	}
}
