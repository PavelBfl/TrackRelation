using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Collections
{
	public class ListTrack<T> : ObjectTrack, IList<T>
	{
		public ListTrack(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
		{
			EqualityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			AddRange(items);
		}
		public ListTrack()
			: this(Enumerable.Empty<T>(), EqualityComparer<T>.Default)
		{

		}
		public ListTrack(IEnumerable<T> items)
			: this (items, EqualityComparer<T>.Default)
		{

		}
		public ListTrack(IEqualityComparer<T> equalityComparer)
			: this(Enumerable.Empty<T>(), equalityComparer)
		{

		}


		private List<T> Items { get; } = new List<T>();
		private List<KeyLife<int, T>> Track { get; } = new List<KeyLife<int, T>>();
		private IEqualityComparer<T> EqualityComparer { get; }

		public T this[int index]
		{
			get => Items[index];
			set
			{
				var currentValue = Items[index];
				if (!EqualityComparer.Equals(currentValue, value))
				{
					var key = KeyProvider.GetNewKey();
					Items[index] = value;
					Track[index].SetItem(value, key);
				}
			}
		}

		public int Count => Items.Count;

		public bool IsReadOnly => false;

		public void Add(T item)
		{
			Add(item, KeyProvider.GetNewKey());
		}
		public void AddRange(IEnumerable<T> items)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}
			if (items.Any())
			{
				var keyTrack = KeyProvider.GetNewKey();
				foreach (var item in items)
				{
					Add(item, keyTrack);
				}
			}
		}
		private void Add(T item, int keyTrack)
		{
			Items.Add(item);
			Track.Add(new KeyLife<int, T>(Items.Count - 1, item, keyTrack));
		}

		public void Clear()
		{
			if (this.Any())
			{
				var key = KeyProvider.GetNewKey();
				foreach (var lifeTime in Track)
				{
					lifeTime.Close(key);
				}
				Items.Clear();
			}
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
			var key = KeyProvider.GetNewKey();
			Items.Insert(index, item);
			for (int i = index; i < Items.Count; i++)
			{
				Track[i].SetItem(Items[i], key);
			}
		}

		public bool Remove(T item)
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
			var key = KeyProvider.GetNewKey();
			Items.RemoveAt(index);
			for (int i = index; i < Items.Count; i++)
			{
				Track[i].SetItem(Items[i], key);
			}
			Track[Items.Count].Close(key);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		protected override void OffsetData(int key)
		{
			Items.Clear();
			foreach (var item in Track)
			{
				if (item.TryGetValue(key, out var result))
				{
					Items.Add(result);
				}
				else
				{
					return;
				}
			}
		}
	}
}
