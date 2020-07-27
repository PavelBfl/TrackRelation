using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Collections
{
	public class ListTrackObserver<T> : ObjectTrack
	{
		private List<KeyLife<int, T>> Track { get; } = new List<KeyLife<int, T>>();
		private IEqualityComparer<T> EqualityComparer { get; }

		public void Commit(IReadOnlyList<T> list)
		{
			if (list is null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			var keyTrackBuikder = new KeyTrackBuilder(KeyProvider);
			for (int i = 0; i < list.Count; i++)
			{
				var newItem = list[i];
				if (i < Track.Count)
				{
					Track[i].TrySetItem(newItem, EqualityComparer, keyTrackBuikder);
				}
				else
				{
					Track.Add(new KeyLife<int, T>(i, newItem, keyTrackBuikder.GetKey()));
				}
			}
		}

		protected override void OffsetData(int key)
		{
			var result = new List<T>();
			foreach (var keyLife in Track)
			{
				if (keyLife.TryGetValue(key, out var item))
				{
					result.Add(item);
				}
				else
				{
					return;
				}
			}
		}
	}
}
