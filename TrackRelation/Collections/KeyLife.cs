using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Collections
{
	class KeyLife<TKey, TValue>
	{
		public KeyLife(TKey key, TValue item, int keyTrack)
		{
			Key = key;
			SetItem(item, keyTrack);
		}

		public TKey Key { get; }

		public IEnumerable<LifeRange<TValue>> LifeRanges => lifeRanges;
		private List<LifeRange<TValue>> lifeRanges = new List<LifeRange<TValue>>();

		public void SetItem(TValue item, int keyTrack)
		{
			Close(keyTrack);
			lifeRanges.Add(new LifeRange<TValue>(keyTrack, item));
		}
		public bool TrySetItem(TValue item, IEqualityComparer<TValue> comparer, KeyTrackBuilder keyTrackBuilder)
		{
			var lastIndex = lifeRanges.Count - 1;
			if (lastIndex >= 0)
			{
				var lastLifeRange = lifeRanges[lastIndex];
				if (!comparer.Equals(item, lastLifeRange.Item))
				{
					SetItem(item, keyTrackBuilder.GetKey());
					return true;
				}
			}
			return false;
		}
		public void Close(int keyTrack)
		{
			var lastIndex = lifeRanges.Count - 1;
			if (lastIndex >= 0)
			{
				var lastLifeRange = lifeRanges[lastIndex];
				if (lastLifeRange.End is null)
				{
					lifeRanges[lastIndex] = lastLifeRange.Close(keyTrack);
				}
			}
		}
		public bool TryClose(KeyTrackBuilder keyTrackBuilder)
		{
			var lastIndex = lifeRanges.Count - 1;
			if (lastIndex >= 0)
			{
				var lastLifeRange = lifeRanges[lastIndex];
				if (lastLifeRange.End is null)
				{
					lifeRanges[lastIndex] = lastLifeRange.Close(keyTrackBuilder.GetKey());
					return true;
				}
			}
			return false;
		}

		public bool TryGetValue(int keyTrack, out TValue result)
		{
			foreach (var item in LifeRanges)
			{
				if (item.Contains(keyTrack))
				{
					result = item.Item;
					return true;
				}
			}

			result = default;
			return false;
		}
	}
}
