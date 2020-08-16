using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Tracks
{
	public class ListTrack<T>
	{
		public ListTrack()
			: this(null)
		{

		}
		public ListTrack(IEqualityComparer<T> comparer)
		{
			Comparer = comparer ?? EqualityComparer<T>.Default;
		}
		public ListTrack(IList<T> list, Transaction transaction, IEqualityComparer<T> comparer)
			: this(comparer)
		{
			Commit(list, transaction);
		}
		public ListTrack(IList<T> list, Transaction transaction)
			: this(null)
		{
			Commit(list, transaction);
		}

		public IEqualityComparer<T> Comparer { get; }

		private List<Track<T>> Track { get; } = new List<Track<T>>();

		public void Commit(IList<T> list, Transaction transaction, IEnumerable<int> indices = null)
		{
			if (list is null)
			{
				throw new ArgumentNullException(nameof(list));
			}
			if (transaction is null)
			{
				throw new ArgumentNullException(nameof(transaction));
			}

			indices = indices ?? Enumerable.Range(0, Math.Max(Track.Count, list.Count));
			foreach (var index in indices)
			{
				if (index < list.Count)
				{
					for (int i = Track.Count; i < list.Count; i++)
					{
						Track.Add(new Track<T>(Comparer));
					}
					Track[index].SetValue(list[index], transaction);
				}
				else
				{
					if (index < list.Count)
					{
						Track[index].Close(transaction);
					}
				}

			} 
		}
		public void Offset(IList<T> list, int key)
		{
			if (list is null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			list.Clear();
			foreach (var keyTrack in Track)
			{
				if (keyTrack.TryGetValue(key, out var item))
				{
					list.Add(item);
				}
				else
				{
					return;
				}
			}
		}
		public void Revert(IList<T> list)
		{
			if (list is null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			list.Clear();
			foreach (var keyTrack in Track)
			{
				if (keyTrack.TryGetLastValue(out var item))
				{
					list.Add(item);
				}
				else
				{
					return;
				}
			}
		}
	}
}
