using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Transact
{
	public class DictionaryTransact<TKey, TValue> : ObjectTransact, IDictionary<TKey, TValue>
	{
		private Dictionary<TKey, TValue> Items { get; } = new Dictionary<TKey, TValue>();
		private Dictionary<TKey, ValueTrack<TValue>> Track { get; } = new Dictionary<TKey, ValueTrack<TValue>>();
		private HashSet<TKey> KeysModified { get; } = new HashSet<TKey>();
		public IEqualityComparer<TValue> Comparer { get; }

		public TValue this[TKey key]
		{
			get => Items[key];
			set
			{
				ThrowIfCommitedEnable();
				if (!Items.TryGetValue(key, out var item) || !Comparer.Equals(value, item))
				{
					KeysModified.Add(key);
					Items[key] = value;
				}
			}
		}

		public ICollection<TKey> Keys => Items.Keys;

		public ICollection<TValue> Values => Items.Values;

		public int Count => Items.Count;

		public bool IsReadOnly => false;


		public void Add(TKey key, TValue value)
		{
			ThrowIfCommitedEnable();
			Items.Add(key, value);
			KeysModified.Add(key);
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			ThrowIfCommitedEnable();
			foreach (var key in Keys)
			{
				KeysModified.Add(key);
			}
			Items.Clear();
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
			ThrowIfCommitedEnable();
			if (Items.Remove(key))
			{
				KeysModified.Add(key);
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

		protected override void CommitData()
		{
			if (KeysModified.Any())
			{
				foreach (var key in KeysModified)
				{
					if (Items.TryGetValue(key, out var item))
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
				KeysModified.Clear();
			}
		}
		protected override void RevertData()
		{
			Items.Clear();
			foreach (var pair in Track)
			{
				if (pair.Value.TryGetLastValue(out var item))
				{
					Items.Add(pair.Key, item);
				}
			}
			KeysModified.Clear();
		}
		protected override void OffsetData(int key)
		{
			Items.Clear();
			foreach (var pair in Track)
			{
				if (pair.Value.TryGetValue(key, out var item))
				{
					Items.Add(pair.Key, item);
				}
			}
			KeysModified.Clear();
		}
	}
}
