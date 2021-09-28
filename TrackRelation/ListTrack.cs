using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation
{
	/// <summary>
	/// Трекер листа
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	public class ListTrack<TKey, TValue>
	{
		public ListTrack()
			: this(null)
		{

		}
		public ListTrack(IComparer<TKey> comparer)
		{
			Comparer = comparer;
		}

		public IComparer<TKey> Comparer { get; }

		/// <summary>
		/// Базовый элемент отслеживания данных
		/// </summary>
		private List<SortedList<TKey, SortedList<TKey, TValue>>> Tracks { get; } = new List<SortedList<TKey, SortedList<TKey, TValue>>>();

		/// <summary>
		/// Зафиксировать данные
		/// </summary>
		/// <param name="list">Список данных</param>
		/// <param name="transaction">Транзакция</param>
		/// <param name="indices">Фиксируемые индексы, если установлено null фиксируются все индексы</param>
		public void Add(TKey key, IReadOnlyList<TValue> list)
		{
			if (list.Count >= Tracks.Count)
			{
				for (int i = 0; i < Tracks.Count; i++)
				{
					Tracks[i].AddTrackRagged(key, list[i]);
				}
				for (int i = Tracks.Count; i < list.Count; i++)
				{
					var track = new SortedList<TKey, SortedList<TKey, TValue>>(Comparer);
					track.AddTrackRagged(key, list[i]);
					Tracks.Add(track);
				}
			}
			else
			{
				for (int i = 0; i < list.Count; i++)
				{
					Tracks[i].AddTrackRagged(key, list[i]);
				}
				for (int i = list.Count; i < Tracks.Count; i++)
				{
					Tracks[i].CloseTrackRagged(key);
				}
			}
		}
	}
}
