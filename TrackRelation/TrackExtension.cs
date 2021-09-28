using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

		public static int IndexOfKeyTrack<TKey, TValue>(this SortedList<TKey, TValue> list, TKey key)
			=> BinsrySearch(new ReadOnlyCollection<TKey>(list.Keys), key, x => x, list.Comparer);
		public static bool TryGetValueTrack<TKey, TValue>(this SortedList<TKey, TValue> list, TKey key, out TValue result)
		{
			var index = list.IndexOfKeyTrack(key);
			if (index >= 0)
			{
				result = list.Values[index];
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}
		public static TValue GetValueTrack<TKey, TValue>(this SortedList<TKey, TValue> list, TKey key)
		{
			var index = list.IndexOfKeyTrack(key);
			return list.Values[index];
		}


		public static void CloseTrackRagged<TKey, TValue>(this SortedList<TKey, SortedList<TKey, TValue>> list, TKey key)
		{
			if (list.Any() && !(list.Values.Last() is null))
			{
				list.Add(key, null);
			}
		}
		public static void AddTrackRagged<TKey, TValue>(this SortedList<TKey, SortedList<TKey, TValue>> list, TKey key, TValue value)
		{
			var subTrack = list.Values.LastOrDefault();
			if (subTrack is null)
			{
				subTrack = new SortedList<TKey, TValue>(list.Comparer);
			}
			subTrack.Add(key, value);
		}
	}
}
