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

			if (items.Any())
			{
				var key = KeyProvider.GetNewKey();
				foreach (var item in items)
				{
					var lifeData = CreateLifeTime(key, Items.Count, item);
					Items.Add(lifeData);
				} 
			}
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


		private List<LifeTime> Items { get; } = new List<LifeTime>();
		private List<LifeTime> AllItems { get; } = new List<LifeTime>();
		private IEqualityComparer<T> EqualityComparer { get; }

		private LifeTime CreateLifeTime(int key, int index, T item)
		{
			var result = new LifeTime(key, index, item);
			AllItems.Add(result);
			return result;
		}

		public T this[int index]
		{
			get => Items[index].Item;
			set
			{
				var currentValue = Items[index];
				if (!EqualityComparer.Equals(currentValue.Item, value))
				{
					var key = KeyProvider.GetNewKey();
					currentValue.Close(key);
					var newValue = CreateLifeTime(key, index, value);
					Items[index] = newValue;
				}
			}
		}

		public int Count => Items.Count;

		public bool IsReadOnly => false;

		public void Add(T item)
		{
			var lifeTime = CreateLifeTime(KeyProvider.GetNewKey(), Items.Count, item);
			Items.Add(lifeTime);
		}

		public void Clear()
		{
			if (this.Any())
			{
				var key = KeyProvider.GetNewKey();
				foreach (var lifeTime in Items)
				{
					lifeTime.Close(key);
				}
				Items.Clear();
			}
		}

		public bool Contains(T item)
		{
			return this.AsEnumerable().Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.ToList().CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Items.Select(item => item.Item).GetEnumerator();
		}

		public int IndexOf(T item)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (EqualityComparer.Equals(Items[i].Item, item))
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(int index, T item)
		{
			var key = KeyProvider.GetNewKey();
			var lifeTime = CreateLifeTime(key, index, item);
			Items.Insert(index, lifeTime);
			for (int i = index + 1; i < Items.Count; i++)
			{
				Items[i].SetIndex(key, i);
			}
		}

		public bool Remove(T item)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (EqualityComparer.Equals(Items[i].Item, item))
				{
					RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			var key = KeyProvider.GetNewKey();
			Items[index].Close(key);
			Items.RemoveAt(index);
			for (int i = index; i < Items.Count; i++)
			{
				Items[i].SetIndex(key, i);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private class LifeTime
		{
			public LifeTime(int begin, int index, T item)
			{
				lifeData.Add(new LifeData(begin, index));
				Item = item;
			}

			public IEnumerable<LifeData> LifeData => lifeData;
			private readonly List<LifeData> lifeData = new List<LifeData>();

			public void SetIndex(int key, int index)
			{
				Close(key);
				lifeData.Add(new LifeData(key, index));
			}
			public void Close(int key)
			{
				var lastIndex = lifeData.Count - 1;
				lifeData[lastIndex] = lifeData[lastIndex].SetEnd(key);
			}

			public T Item { get; }
		}
		private struct LifeData
		{
			public LifeData(int begin, int? end, int index)
			{
				Begin = begin;
				End = end;
				Index = index;
			}
			public LifeData(int begin, int index)
				: this(begin, null, index)
			{
				
			}

			public int Begin { get; }
			public int? End { get; }
			public int Index { get; }

			public LifeData SetEnd(int end)
			{
				return new LifeData(Begin, end, Index);
			}
		}
	}
}
