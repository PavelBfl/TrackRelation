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
		public ListTransact(IEnumerable<TValue> items, IEqualityComparer<TValue> equalityComparer, DispatcherTrack<TKey> dispatcher)
			: base(dispatcher)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}
			ListObserver = new ListObserver<TKey, TValue, List<TValue>>(new List<TValue>(), equalityComparer, dispatcher);

			AddRange(items);
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
				ThrowIfCommitedEnable();
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
			ThrowIfCommitedEnable();
			Items.Add(item);
			Indiсes.Add(Items.Count - 1);
		}
		public void AddRange(IEnumerable<TValue> items)
		{
			ThrowIfCommitedEnable();
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
			ThrowIfCommitedEnable();
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
			ThrowIfCommitedEnable();
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
			ThrowIfCommitedEnable();
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

		protected override void CommitData()
		{
			ListObserver.Commit(Indiсes);
		}
		protected override void RevertData()
		{
			ListObserver.Revert();
			Indiсes.Clear();
		}
		protected override void OffsetData(TKey key)
		{
			ListObserver.Offset(key);
			Indiсes.Clear();
		}
	}
}
