using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Tracks
{
	public class ArrayTrack<TKey, TValue>
	{
		public ArrayTrack(int size, IComparer<TKey> comparer)
		{
			Tracks = new SortedList<TKey, TValue>[size];
			for (int i = 0; i < Tracks.Length; i++)
			{
				Tracks[i] = new SortedList<TKey, TValue>(comparer);
			}
		}

		private SortedList<TKey, TValue>[] Tracks { get; }

		public void Add(TKey key, IReadOnlyList<TValue> items)
		{
			if (Tracks.Length != items.Count)
			{
				throw new InvalidOperationException();
			}

			for (int i = 0; i < Tracks.Length; i++)
			{
				Tracks[i].Add(key, items[i]);
			}
		}
		public bool TryGetValue(TKey key, out TValue[] result)
		{
			result = new TValue[Tracks.Length];
			for (int i = 0; i < Tracks.Length; i++)
			{
				if (Tracks[i].TryGetValueTrack(key, out var item))
				{
					result[i] = item;
				}
				else
				{
					result = null;
					return false;
				}
			}
			return true;
		}

		public void Remove(TKey key)
		{
			foreach (var track in Tracks)
			{
				track.RemoveTrack(key);
			}
		}
	}
}
