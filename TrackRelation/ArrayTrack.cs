using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Tracks
{
	public class ArrayTrack<TKey, TValue>
	{
		public ArrayTrack(int size, IParameters<TKey, TValue> parameters)
		{
			Parameters = parameters ?? new Parameters<TKey, TValue>();

			Tracks = new Track<TKey, TValue>[size];
			for (int i = 0; i < Tracks.Length; i++)
			{
				Tracks[i] = new Track<TKey, TValue>(Parameters);
			}
		}

		public IParameters<TKey, TValue> Parameters { get; }

		private Track<TKey, TValue>[] Tracks { get; }

		public void Add(TKey key, IReadOnlyList<TValue> items)
		{
			if (Tracks.Length != items.Count)
			{
				throw new InvalidOperationException();
			}

			for (int i = 0; i < Tracks.Length; i++)
			{
				Tracks[i].SetValue(key, items[i]);
			}
		}
		public bool TryGetValue(TKey key, out TValue[] result)
		{
			result = new TValue[Tracks.Length];
			for (int i = 0; i < Tracks.Length; i++)
			{
				if (Tracks[i].TryGetValue(key, out var item))
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
	}
}
