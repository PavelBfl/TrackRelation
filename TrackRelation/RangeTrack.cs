using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	struct RangeTrack<T>
	{
		private RangeTrack(int begin, int? end, T value)
		{
			if (begin >= end)
			{
				throw new ArgumentOutOfRangeException(nameof(end));
			}

			Begin = begin;
			End = end;
			Value = value;
		}
		public RangeTrack(int begin, T value)
			: this(begin, null, value)
		{
			
		}
		public RangeTrack(int begin, int end, T value)
			: this(begin, new int?(end), value)
		{

		}

		public int Begin { get; }
		public int? End { get; }
		public T Value { get; }

		public bool Contains(int key)
		{
			return Begin <= key && key < (End ?? int.MaxValue);
		}
		public RangeTrack<T> Close(int end)
		{
			return new RangeTrack<T>(Begin, end, Value);
		}
	}
}
