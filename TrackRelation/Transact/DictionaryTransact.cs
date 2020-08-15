using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Track.Relation.Transact
{
	public class DictionaryTransact<TKey, TValue> : ObjectTransact, IDictionary<TKey, TValue>
	{
		private DictionaryObserver<TKey, TValue, Dictionary<TKey, TValue>> DictionaryObserver { get; } = new DictionaryObserver<TKey, TValue, Dictionary<TKey, TValue>>()
		{
			Dictionary = new Dictionary<TKey, TValue>(),
		};
		private Dictionary<TKey, TValue> Items => DictionaryObserver.Dictionary;

		private HashSet<TKey> Indices { get; } = new HashSet<TKey>();


		public TValue this[TKey key]
		{
			get => Items[key];
			set
			{
				ThrowIfCommitedEnable();
				if (!Items.TryGetValue(key, out var item) || !Comparer.Equals(value, item))
				{
					Indices.Add(key);
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
			Indices.Add(key);
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
				Indices.Add(key);
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
				Indices.Add(key);
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
			DictionaryObserver.Commit(Indices);
			Indices.Clear();
		}
		protected override void RevertData()
		{
			DictionaryObserver.Revert();
			Indices.Clear();
		}
		protected override void OffsetData(int key)
		{
			DictionaryObserver.Offset(key);
			Indices.Clear();
		}
	}
}
