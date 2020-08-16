using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Track.Relation.Tracks;

namespace Track.Relation.Tracks
{
	/// <summary>
	/// Трекер словаря
	/// </summary>
	/// <typeparam name="TKey">Тип ключа словаря</typeparam>
	/// <typeparam name="TValue">Тип значения словаря</typeparam>
	public class DictionaryTrack<TKey, TValue>
	{
		public DictionaryTrack(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
		{
			Track = new Dictionary<TKey, Track<TValue>>(keyComparer);
			Comparer = valueComparer ?? EqualityComparer<TValue>.Default;
		}
		public DictionaryTrack(IEqualityComparer<TKey> keyComparer)
			: this(keyComparer, null)
		{

		}
		public DictionaryTrack(IEqualityComparer<TValue> valueComparer)
			: this(null, valueComparer)
		{

		}
		public DictionaryTrack()
			: this(null, null)
		{

		}

		/// <summary>
		/// Базовый трекер
		/// </summary>
		private Dictionary<TKey, Track<TValue>> Track { get; }
		/// <summary>
		/// Объект сравнения данных
		/// </summary>
		public IEqualityComparer<TValue> Comparer { get; }

		/// <summary>
		/// Зафиксировать данные
		/// </summary>
		/// <param name="dictionary">Контейнер данных</param>
		/// <param name="transaction">Транзакция</param>
		/// <param name="indices">Фиксируемые индексы, если указан null коминтируются все индексы</param>
		public void Commit(IDictionary<TKey, TValue> dictionary, Transaction transaction, IEnumerable<TKey> indices = null)
		{
			if (dictionary is null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}
			if (transaction is null)
			{
				throw new ArgumentNullException(nameof(transaction));
			}

			indices = indices ?? Track.Keys.ToArray();
			foreach (var key in indices)
			{
				if (dictionary.TryGetValue(key, out var item))
				{
					if (Track.TryGetValue(key, out var keyTrack))
					{
						keyTrack.SetValue(item, transaction);
					}
					else
					{
						Track.Add(key, new Track<TValue>(item, Comparer, transaction));
					}
				}
				else
				{
					if (Track.TryGetValue(key, out var keyTrack))
					{
						keyTrack.Close(transaction);
					}
				}
			}
		}
		/// <summary>
		/// Получить данные ревизии
		/// </summary>
		/// <param name="dictionary">Контейнер данных</param>
		/// <param name="key">Ключ ревизии</param>
		public void Offset(IDictionary<TKey, TValue> dictionary, int key)
		{
			if (dictionary is null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			dictionary.Clear();
			foreach (var pair in Track)
			{
				if (pair.Value.TryGetValue(key, out var item))
				{
					dictionary.Add(pair.Key, item);
				}
			}
		}
		/// <summary>
		/// Получить последние данные
		/// </summary>
		/// <param name="dictionary">Контейнер данных</param>
		public void Revert(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary is null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			dictionary.Clear();
			foreach (var pair in Track)
			{
				if (pair.Value.TryGetLastValue(out var item))
				{
					dictionary.Add(pair.Key, item);
				}
			}
		}
	}
}
