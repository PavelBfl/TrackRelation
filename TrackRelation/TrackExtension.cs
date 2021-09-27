using System;
using System.Collections.Generic;

namespace Track.Relation
{
	public static class TrackExtension
	{
		public static int BinsrySearch<TItem, TKey>(this IReadOnlyList<TItem> list, TKey key, Func<TItem, TKey> keyGetter, IComparer<TKey> comparer)
		{
			if (list.Count > 0)
			{
				var begin = 0;
				var end = list.Count - 1;

				if (comparer.Compare(keyGetter(list[begin]), key) > 0)
				{
					return -1;
				}
				else if (comparer.Compare(keyGetter(list[end]), key) <= 0)
				{
					return end;
				}

				while (true)
				{
					var current = (end - begin) / 2 + begin;
					var compare = comparer.Compare(keyGetter(list[current]), key);

					if (compare > 0)
					{
						end = current;
					}
					else if (compare < 0)
					{
						if (begin != current)
						{
							begin = current;
						}
						else
						{
							return current;
						}
					}
					else
					{
						return current;
					}
				}
			}
			else
			{
				return -1;
			}
		}
	}
}
