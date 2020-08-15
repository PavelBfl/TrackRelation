using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation
{
	public class ListCommitter<T> : ObjectTrack
	{
		public IEqualityComparer<T> Comparer { get; }

		private List<ValueTrack<T>> Track { get; } = new List<ValueTrack<T>>();

		public void Commit(IList<T> list, IEnumerable<int> indices = null)
		{
			if (list is null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			using (new LocalTransaction(DispatcherTrack))
			{
				indices = indices ?? Enumerable.Range(0, Track.Count);
				foreach (var index in indices)
				{
					if (index < list.Count)
					{
						for (int i = Track.Count; i < list.Count; i++)
						{
							Track.Add(new ValueTrack<T>(Comparer));
						}
						Track[index].SetValue(list[index], DispatcherTrack.Transaction);
					}
					else
					{
						if (index < list.Count)
						{
							Track[index].Close(DispatcherTrack.Transaction);
						}
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
