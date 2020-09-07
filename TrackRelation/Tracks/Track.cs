using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Tracks
{
	/// <summary>
	/// Поток отслеживания значения
	/// </summary>
	/// <typeparam name="TValue">Тип отслеживаемого значения</typeparam>
	public class Track<TKey, TValue>
	{
		public Track()
			: this(null)
		{
			
		}
		public Track(TValue value, Transaction<TKey> transaction)
			: this(value, null, transaction)
		{
			SetValue(value, transaction);
		}
		public Track(TValue value, IEqualityComparer<TValue> comparer, Transaction<TKey> transaction)
			: this(comparer)
		{
			SetValue(value, transaction);
		}
		public Track(IEqualityComparer<TValue> comparer)
		{
			Comparer = comparer ?? EqualityComparer<TValue>.Default;
		}

		/// <summary>
		/// Список диапазонов изменения значений
		/// </summary>
		private readonly List<RangeTrack<TKey, TValue>> track = new List<RangeTrack<TKey, TValue>>();
		/// <summary>
		/// Объект сравнения значений
		/// </summary>
		public IEqualityComparer<TValue> Comparer { get; }

		/// <summary>
		/// Назначить новое значение
		/// </summary>
		/// <param name="value">Новое значение</param>
		/// <param name="transaction">Транзакция</param>
		/// <returns>true если новое значение было сохранено, иначе false</returns>
		public bool SetValue(TValue value, Transaction<TKey> transaction)
		{
			if (!TryGetLastRange(out var range) || !Comparer.Equals(value, range.Value))
			{
				CheckKey(range, transaction);
				Close(transaction);
				track.Add(new RangeTrack<TKey, TValue>(transaction.Key, value));
				return true;
			}
			return false;
		}
		/// <summary>
		/// Закрыть существование значения
		/// </summary>
		/// <param name="transaction">Транзакция</param>
		public void Close(Transaction<TKey> transaction)
		{
			if (TryGetLastRange(out var lastRange))
			{
				CheckKey(lastRange, transaction);
				if (new Comparable<TKey>(lastRange.End).IsDefault())
				{
					track[track.Count - 1] = lastRange.Close(transaction.Key);
				}
			}
		}

		/// <summary>
		/// Проверить корректность предоставляемого ключа
		/// </summary>
		/// <param name="range">Последний диапазон значения</param>
		/// <param name="transaction">Транзакция</param>
		private static void CheckKey(RangeTrack<TKey, TValue> range, Transaction<TKey> transaction)
		{
			var end = new Comparable<TKey>(range.End);
			var currentKey = end.IsDefault() ? new Comparable<TKey>(range.Begin) : end;

			if (currentKey >= new Comparable<TKey>(transaction.Key))
			{
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Получить значение соответствующее ключу
		/// </summary>
		/// <param name="key">Ключ значения</param>
		/// <param name="result">Результирующее значение</param>
		/// <returns>true если значение удалось найти, наче false</returns>
		public bool TryGetValue(TKey key, out TValue result)
		{
			foreach (var range in track)
			{
				if (range.Contains(key))
				{
					result = range.Value;
					return true;
				}
			}

			result = default;
			return false;
		}
		/// <summary>
		/// Получить последнее существующее значение
		/// </summary>
		/// <param name="result">Результрующее значение</param>
		/// <returns>true если значение удалось получить, иначе false</returns>
		public bool TryGetLastValue(out TValue result)
		{
			var tryResult = TryGetLastRange(out var lastRange);
			result = lastRange.Value;
			return tryResult;
		}

		/// <summary>
		/// Получить последний диапазон значения
		/// </summary>
		/// <param name="result">Результирующий диапазон</param>
		/// <returns>True если удалось вернуть последний диапазон, иначе false</returns>
		private bool TryGetLastRange(out RangeTrack<TKey, TValue> result)
		{
			var lastIndex = track.Count - 1;
			if (lastIndex >= 0)
			{
				var lastRange = track[lastIndex];
				if (new Comparable<TKey>(lastRange.End).IsDefault())
				{
					result = lastRange;
					return true;
				}
			}

			result = default;
			return false;
		}
	}
}
