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
		public ListTrack(IParameters<TKey, TValue> parameters)
		{
			Parameters = parameters ?? new Parameters<TKey, TValue>();
		}

		public IParameters<TKey, TValue> Parameters { get; }

		/// <summary>
		/// Базовый элемент отслеживания данных
		/// </summary>
		private List<TrackRagged<TKey, TValue>> Tracks { get; } = new List<TrackRagged<TKey, TValue>>();

		/// <summary>
		/// Зафиксировать данные
		/// </summary>
		/// <param name="list">Список данных</param>
		/// <param name="transaction">Транзакция</param>
		/// <param name="indices">Фиксируемые индексы, если установлено null фиксируются все индексы</param>
		public void Commit(TKey key, IReadOnlyList<TValue> list)
		{
			if (list.Count >= Tracks.Count)
			{
				for (int i = 0; i < Tracks.Count; i++)
				{
					Tracks[i].Add(key, list[i]);
				}
				for (int i = Tracks.Count; i < list.Count; i++)
				{
					var track = new TrackRagged<TKey, TValue>(Parameters);
					track.Add(key, list[i]);
					Tracks.Add(track);
				}
			}
			else
			{
				for (int i = 0; i < list.Count; i++)
				{
					Tracks[i].Add(key, list[i]);
				}
				for (int i = list.Count; i < Tracks.Count; i++)
				{
					Tracks[i].Close(key);
				}
			}
		}
	}
}
