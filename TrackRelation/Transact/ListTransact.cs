using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Transact
{
	public class ListTransact<T> : ObjectTransact, IList<T>
	{
		public ListTransact(DispatcherTrack dispatcherTrack)
			: this(Enumerable.Empty<T>(), EqualityComparer<T>.Default, dispatcherTrack)
		{
			
		}
		public ListTransact(IEnumerable<T> items, DispatcherTrack dispatcherTrack)
			: this(items, EqualityComparer<T>.Default, dispatcherTrack)
		{

		}
		public ListTransact(IEqualityComparer<T> equalityComparer, DispatcherTrack dispatcherTrack)
			: this(Enumerable.Empty<T>(), equalityComparer, dispatcherTrack)
		{

		}
		public ListTransact(IEnumerable<T> items, IEqualityComparer<T> equalityComparer, DispatcherTrack dispatcherTrack)
			: base(dispatcherTrack)
		{
			Comparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			AddRange(items);
		}
		public ListTransact()
			: this(Enumerable.Empty<T>(), EqualityComparer<T>.Default, DispatcherTrack.Default)
		{

		}
		public ListTransact(IEnumerable<T> items)
			: this (items, EqualityComparer<T>.Default, DispatcherTrack.Default)
		{

		}
		public ListTransact(IEqualityComparer<T> equalityComparer)
			: this(Enumerable.Empty<T>(), equalityComparer, DispatcherTrack.Default)
		{

		}


		private List<T> Items { get; } = new List<T>();
		private List<ValueTrack<T>> Track { get; } = new List<ValueTrack<T>>();
		private HashSet<int> Indiсes { get; } = new HashSet<int>();

		public IEqualityComparer<T> Comparer { get; }

		public T this[int index]
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

		public void Add(T item)
		{
			ThrowIfCommitedEnable();
			Items.Add(item);
			Indiсes.Add(Items.Count - 1);
		}
		public void AddRange(IEnumerable<T> items)
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

		public bool Contains(T item)
		{
			return Items.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Items.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			return Items.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			Items.Insert(index, item);
			for (int i = index; i < Items.Count; i++)
			{
				Indiсes.Add(i);
			}
		}

		public bool Remove(T item)
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
			if (Indiсes.Any())
			{
				foreach (var index in Indiсes)
				{
					if (index < Items.Count)
					{
						for (int i = Track.Count; i < Items.Count; i++)
						{
							Track.Add(new ValueTrack<T>(Comparer));
						}
						Track[index].SetValue(Items[index], DispatcherTrack.Transaction);
					}
					else
					{
						if (index < Items.Count)
						{
							Track[index].Close(DispatcherTrack.Transaction);
						}
					}

				}

				Indiсes.Clear();
			}
		}
		protected override void RevertData()
		{
			Items.Clear();
			foreach (var keyTrack in Track)
			{
				if (keyTrack.TryGetLastValue(out var item))
				{
					Items.Add(item);
				}
				else
				{
					return;
				}
			}
			Indiсes.Clear();
		}
		protected override void OffsetData(int key)
		{
			Items.Clear();
			foreach (var keyTrack in Track)
			{
				if (keyTrack.TryGetValue(key, out var item))
				{
					Items.Add(item);
				}
				else
				{
					return;
				}
			}
			Indiсes.Clear();
		}
	}
}
