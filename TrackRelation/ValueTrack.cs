using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation
{
	class ValueTrack<TValue>
	{
		public ValueTrack()
		{
			
		}
		public ValueTrack(TValue value, KeyBatch keyBatch)
		{
			SetValue(value, keyBatch);
		}

		private readonly List<RangeTrack<TValue>> track = new List<RangeTrack<TValue>>();

		public bool TrySetValue(TValue value, IEqualityComparer<TValue> comparer, KeyBatch keyBatch)
		{
			if (comparer is null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}
			if (!TryGetLastValue(out var lastValue) || !comparer.Equals(value, lastValue))
			{
				SetValue(value, keyBatch);
				return true;
			}
			return false;
		}
		public void SetValue(TValue value, KeyBatch keyBatch)
		{
			Close(keyBatch);
			track.Add(new RangeTrack<TValue>(keyBatch.Key, value));
		}
		public void Close(KeyBatch keyBatch)
		{
			var lastIndex = track.Count - 1;
			if (lastIndex >= 0)
			{
				var lastRange = track[lastIndex];
				if (lastRange.End is null)
				{
					track[lastIndex] = lastRange.Close(keyBatch.Key);
				}
			}
		}

		public bool TryGetValue(int key, out TValue result)
		{
			foreach (var range in track)
			{
				if (range.Contains(key))
				{
					result = range.Value;
					return true;
				}
			}

			result = default;
			return false;
		}
		public bool TryGetLastValue(out TValue result)
		{
			var lastIndex = track.Count - 1;
			if (lastIndex >= 0)
			{
				var lastRange = track[lastIndex];
				if (lastRange.End is null)
				{
					result = lastRange.Value;
					return true;
				}
			}

			result = default;
			return false;
		}
	}
}
