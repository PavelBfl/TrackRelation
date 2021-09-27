using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Track.Relation.Tracks;

namespace Track.Relation
{
	/// <summary>
	/// Трекер словаря
	/// </summary>
	/// <typeparam name="TKey">Тип ключа словаря</typeparam>
	/// <typeparam name="TValue">Тип значения словаря</typeparam>
	public class DictionaryTrack<TCommitKey, TKey, TValue>
	{
		public DictionaryTrack(IEqualityComparer<TKey> keyComparer, IParameters<TCommitKey, TValue> parameters)
		{
			Tracks = new Dictionary<TKey, TrackRagged<TCommitKey, TValue>>(keyComparer);
			Parameters = parameters;
		}

		/// <summary>
		/// Базовый трекер
		/// </summary>
		private Dictionary<TKey, TrackRagged<TCommitKey, TValue>> Tracks { get; }
		public IParameters<TCommitKey, TValue> Parameters { get; }

		/// <summary>
		/// Зафиксировать данные
		/// </summary>
		/// <param name="dictionary">Контейнер данных</param>
		/// <param name="transaction">Транзакция</param>
		/// <param name="indices">Фиксируемые индексы, если указан null коминтируются все индексы</param>
		public void Commit(TCommitKey key, IReadOnlyDictionary<TKey, TValue> dictionary)
		{
			foreach (var valueKey in dictionary.Keys.Union(Tracks.Keys))
			{
				if (Tracks.TryGetValue(valueKey, out var track))
				{
					if (dictionary.TryGetValue(valueKey, out var value))
					{
						track.Add(key, value);
					}
					else
					{
						track.Close(key);
					}
				}
				else
				{
					if (dictionary.TryGetValue(valueKey, out var value))
					{
						var newTrack = new TrackRagged<TCommitKey, TValue>(Parameters);
						newTrack.Add(key, value);
						Tracks.Add(valueKey, newTrack);
					}
					else
					{
						throw new InvalidOperationException();
					}
				}
			}
		}
	}
}
