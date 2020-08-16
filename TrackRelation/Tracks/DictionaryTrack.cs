using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Track.Relation.Tracks;

namespace Track.Relation.Tracks
{
	public class DictionaryTrack<TKey, TValue>
	{
		private Dictionary<TKey, Track<TValue>> Track { get; } = new Dictionary<TKey, Track<TValue>>();
		public IEqualityComparer<TValue> Comparer { get; }

		public void Commit(IDictionary<TKey, TValue> dictionary, Transaction transaction, IEnumerable<TKey> indices = null)
		{
			if (dictionary is null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}
			if (transaction is null)
			{
				throw new ArgumentNullException(nameof(transaction));
			}

			indices = indices ?? Track.Keys.ToArray();
			foreach (var key in indices)
			{
				if (dictionary.TryGetValue(key, out var item))
				{
					if (Track.TryGetValue(key, out var keyTrack))
					{
						keyTrack.SetValue(item, transaction);
					}
					else
					{
						Track.Add(key, new Track<TValue>(item, Comparer, transaction));
					}
				}
				else
				{
					if (Track.TryGetValue(key, out var keyTrack))
					{
						keyTrack.Close(transaction);
					}
				}
			}
		}
		public void Offset(IDictionary<TKey, TValue> dictionary, int key)
		{
			if (dictionary is null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			dictionary.Clear();
			foreach (var pair in Track)
			{
				if (pair.Value.TryGetValue(key, out var item))
				{
					dictionary.Add(pair.Key, item);
				}
			}
		}
		public void Revert(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary is null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			dictionary.Clear();
			foreach (var pair in Track)
			{
				if (pair.Value.TryGetLastValue(out var item))
				{
					dictionary.Add(pair.Key, item);
				}
			}
		}
	}
}
