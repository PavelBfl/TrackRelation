using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Track.Relation.Transact
{
	/// <summary>
	/// Словарь отслеживаемых данных
	/// </summary>
	/// <typeparam name="TKey">Тип ключа</typeparam>
	/// <typeparam name="TValue">Тип значения</typeparam>
	public class DictionaryTransact<TCommitKey, TKey, TValue> : ObjectTransact<TCommitKey>, IDictionary<TKey, TValue>
	{
		public DictionaryTransact(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, DispatcherTrack<TCommitKey> dispatcher)
			: base(dispatcher)
		{
			DictionaryObserver = new DictionaryObserver<TCommitKey, TKey, TValue, Dictionary<TKey, TValue>>(new Dictionary<TKey, TValue>(keyComparer), keyComparer, valueComparer, dispatcher);
			Indices = new HashSet<TKey>(keyComparer);
		}

		/// <summary>
		/// Наблюдатель за изменениями
		/// </summary>
		private DictionaryObserver<TCommitKey, TKey, TValue, Dictionary<TKey, TValue>> DictionaryObserver { get; }
		/// <summary>
		/// Словарь данных
		/// </summary>
		private Dictionary<TKey, TValue> Items => DictionaryObserver.Dictionary;
		/// <summary>
		/// Изменённые индексы
		/// </summary>
		private HashSet<TKey> Indices { get; }


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
		protected override void OffsetData(TCommitKey key)
		{
			DictionaryObserver.Offset(key);
			Indices.Clear();
		}
	}
}
