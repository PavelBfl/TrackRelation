using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Committers
{
	public class DictionaryCommitter<TKey, TValue> : ObjectTrack
	{
		private Dictionary<TKey, ValueTrack<TValue>> Track { get; } = new Dictionary<TKey, ValueTrack<TValue>>();
		public IEqualityComparer<TValue> Comparer { get; }

		public void Commit(IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> indices = null)
		{
			if (dictionary is null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			using (new LocalTransaction(DispatcherTrack))
			{
				indices = indices ?? Track.Keys.ToArray();
				foreach (var key in indices)
				{
					if (dictionary.TryGetValue(key, out var item))
					{
						if (Track.TryGetValue(key, out var keyTrack))
						{
							keyTrack.SetValue(item, DispatcherTrack.Transaction);
						}
						else
						{
							Track.Add(key, new ValueTrack<TValue>(item, Comparer, DispatcherTrack.Transaction));
						}
					}
					else
					{
						if (Track.TryGetValue(key, out var keyTrack))
						{
							keyTrack.Close(DispatcherTrack.Transaction);
						}
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
