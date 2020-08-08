using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Transact
{
	class ListIndices<T> : IDictionary<int, T>
	{
		private List<T> Items { get; } = new List<T>();

		public T this[int key] { get => Items[key]; set => Items[key] = value; }

		public ICollection<int> Keys => Enumerable.Range(0, Items.Count).ToArray();

		public ICollection<T> Values => Items;

		public int Count => Items.Count;

		public bool IsReadOnly => false;


		public void Add(int key, T value)
		{
			
		}

		public void Add(KeyValuePair<int, T> item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<int, T> item)
		{
			throw new NotImplementedException();
		}

		public bool ContainsKey(int key)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<int, T>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public bool Remove(int key)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<int, T> item)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(int key, out T value)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
