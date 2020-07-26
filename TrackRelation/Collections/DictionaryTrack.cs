using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Collections
{
	public class DictionaryTrack<TKey, TValue> : ObjectTrack, IDictionary<TKey, TValue>
	{
		private Dictionary<TKey, TValue> Items { get; } = new Dictionary<TKey, TValue>();
		private Dictionary<TKey, KeyLife<TKey, TValue>> Track { get; } = new Dictionary<TKey, KeyLife<TKey, TValue>>();
		private IEqualityComparer<TValue> EqualityComparer { get; }

		public TValue this[TKey key]
		{
			get => Items[key];
			set
			{
				var item = Items[key];
				if (!EqualityComparer.Equals(item, value))
				{
					Items[key] = value;
					var keyTrack = KeyProvider.GetNewKey();
					if (Track.TryGetValue(key, out var keyLife))
					{
						keyLife.SetItem(value, keyTrack);
					}
					else
					{
						Track.Add(key, new KeyLife<TKey, TValue>(key, value, keyTrack));
					}
				}
			}
		}

		public ICollection<TKey> Keys => Items.Keys;

		public ICollection<TValue> Values => Items.Values;

		public int Count => Items.Count;

		public bool IsReadOnly => false;


		public void Add(TKey key, TValue value)
		{
			Items.Add(key, value);
			var keyTrack = KeyProvider.GetNewKey();
			Track.Add(key, new KeyLife<TKey, TValue>(key, value, keyTrack));
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			if (Items.Any())
			{
				Items.Clear();
				var keyTrack = KeyProvider.GetNewKey();
				foreach (var keyLife in Track.Values)
				{
					keyLife.Close(keyTrack);
				} 
			}
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>)Items).Contains(item);
		}

		public bool ContainsKey(TKey key)
		{
			return Items.ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)Items).CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		public bool Remove(TKey key)
		{
			if (Items.Remove(key))
			{
				var keyTrack = KeyProvider.GetNewKey();
				Track[key].Close(keyTrack);
				return true;
			}

			return false;
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return Items.TryGetValue(key, out value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		protected override void OffsetData(int key)
		{
			Items.Clear();
			foreach (var keyLife in Track.Values)
			{
				if (keyLife.TryGetValue(key, out var result))
				{
					Items.Add(keyLife.Key, result);
				}
			}
		}
	}
}
