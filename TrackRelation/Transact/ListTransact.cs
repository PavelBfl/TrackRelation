using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Transact
{

	/// <summary>
	/// Лист отслеживаемых данных
	/// </summary>
	/// <typeparam name="TValue">Тип эллемента коллекции</typeparam>
	public class ListTransact<TKey, TValue> : ObjectTransact<TKey>, IList<TValue>
	{
		public ListTransact(IEnumerable<TValue> items, IEqualityComparer<TValue> equalityComparer)
			: this(equalityComparer)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}
			AddRange(items);
		}
		public ListTransact(IEqualityComparer<TValue> equalityComparer)
		{
			ListObserver = new ListObserver<TKey, TValue, List<TValue>>(new List<TValue>(), equalityComparer);
		}
		public ListTransact()
			: this(null)
		{

		}

		/// <summary>
		/// Изменёные индексы
		/// </summary>
		private HashSet<int> Indiсes { get; } = new HashSet<int>();
		/// <summary>
		/// Объект наблюдения за коллекцией
		/// </summary>
		private ListObserver<TKey, TValue, List<TValue>> ListObserver { get; }
		/// <summary>
		/// Коллекция данных
		/// </summary>
		private List<TValue> Items => ListObserver.List;

		public TValue this[int index]
		{
			get => Items[index];
			set
			{
				var item = Items[index];
				if (Comparer.Equals(item, value))
				{
					Indiсes.Add(index);
					Items[index] = value;
				}
			}
		}

		public int Count => Items.Count;

		public bool IsReadOnly => false;

		public void Add(TValue item)
		{
			Items.Add(item);
			Indiсes.Add(Items.Count - 1);
		}
		public void AddRange(IEnumerable<TValue> items)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}
			var startIndex = Items.Count;
			Items.AddRange(items);
			for (int i = startIndex; i < Items.Count; i++)
			{
				Indiсes.Add(i);
			}
		}

		public void Clear()
		{
			for (int i = 0; i < Items.Count; i++)
			{
				Indiсes.Add(i);
			}
			Items.Clear();
		}

		public bool Contains(TValue item)
		{
			return Items.Contains(item);
		}

		public void CopyTo(TValue[] array, int arrayIndex)
		{
			Items.CopyTo(array, arrayIndex);
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		public int IndexOf(TValue item)
		{
			return Items.IndexOf(item);
		}

		public void Insert(int index, TValue item)
		{
			Items.Insert(index, item);
			for (int i = index; i < Items.Count; i++)
			{
				Indiсes.Add(i);
			}
		}

		public bool Remove(TValue item)
		{
			var index = Items.IndexOf(item);
			if (index >= 0)
			{
				RemoveAt(index);
				return true;
			}

			return false;
		}

		public void RemoveAt(int index)
		{
			Items.RemoveAt(index);
			for (int i = index; i < Items.Count; i++)
			{
				Indiсes.Add(i);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override void Commit(Transaction<TKey> transaction)
		{
			ListObserver.Commit(Indiсes, transaction);
		}
		public override void Revert()
		{
			ListObserver.Revert();
			Indiсes.Clear();
		}
		public override void Offset(TKey key)
		{
			ListObserver.Offset(key);
			Indiсes.Clear();
		}
	}
}
