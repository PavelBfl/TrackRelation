using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Collections
{
	class DictionaryTrackObserver<TKey, TValue> : ObjectTrack
	{
		private Dictionary<TKey, KeyLife<TKey, TValue>> Track { get; } = new Dictionary<TKey, KeyLife<TKey, TValue>>();
		private IEqualityComparer<TKey> KeyComparer { get; }
		private IEqualityComparer<TValue> ValueComparer { get; }

		public void Commit(IReadOnlyDictionary<TKey, TValue> dictionary)
		{
			if (dictionary is null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			var keyTrackBuilder = new KeyTrackBuilder(KeyProvider);
			foreach (var pair in dictionary)
			{
				if (Track.TryGetValue(pair.Key, out var keyLife))
				{
					keyLife.TrySetItem(pair.Value, ValueComparer, keyTrackBuilder);
				}
				else
				{
					Track.Add(pair.Key, new KeyLife<TKey, TValue>(pair.Key, pair.Value, keyTrackBuilder.GetKey()));
				}
			}
		}

		protected override void OffsetData(int key)
		{
			
		}
	}
}
