using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
		public IEnumerable<ICommit<TKey, TValue>> Commits => ranges.Cast<ICommit<TKey, TValue>>();
		private readonly List<RangeTrack<TKey, TValue>> ranges = new List<RangeTrack<TKey, TValue>>();
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
				ranges.Add(new RangeTrack<TKey, TValue>(transaction.Key, value));
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
					ranges[ranges.Count - 1] = lastRange.Close(transaction.Key);
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
			if (new Comparable<TKey>(key).IsDefault())
			{
				throw new InvalidOperationException();
			}
			foreach (var range in ranges)
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
			var lastIndex = ranges.Count - 1;
			if (lastIndex >= 0)
			{
				var lastRange = ranges[lastIndex];
				if (new Comparable<TKey>(lastRange.End).IsDefault())
				{
					result = lastRange;
					return true;
				}
			}

			result = default;
			return false;
		}

		/// <summary>
		/// Отчистить сохранённые данные
		/// </summary>
		/// <param name="begin">Ключ фиксации с которого удаляются данные</param>
		/// <param name="end">Ключ фиксации до которого удаляются данные</param>
		public void Clear(TKey begin, TKey end)
		{
			var beginComparable = new Comparable<TKey>(begin);
			var endComparable = new Comparable<TKey>(end);
			if (beginComparable < endComparable)
			{
				throw new InvalidOperationException();
			}

			var beginData = FindIndex(begin);
			var endData = FindIndex(end);

			if (beginData.Index >= 0 && endData.Index >= 0)
			{
				ranges.RemoveRange(beginData.Index, endData.Index - beginData.Index);
			}
			else if (beginData.Index >= 0 && endData.Position == TrackPosition.After)
			{
				ranges.RemoveRange(beginData.Index, ranges.Count - beginData.Index);
			}
			else if (beginData.Position == TrackPosition.Before && endData.Index >= 0)
			{
				ranges.RemoveRange(0, endData.Index);
			}
			else if (!((beginData.Position == TrackPosition.Before && endData.Position == TrackPosition.Before) ||
				(beginData.Position == TrackPosition.After && endData.Position == TrackPosition.After)))
			{
				throw new InvalidOperationException();
			}
		}

		private enum TrackPosition
		{
			None,
			Before,
			After,
		}
		private (TrackPosition Position, int Index) FindIndex(TKey key)
		{
			var keyComparable = new Comparable<TKey>(key);
			if (ranges.Any())
			{
				if (ranges.First().CompareTo(key) < 0)
				{
					return (TrackPosition.Before, -1);
				}
				else if (ranges.Last().CompareTo(key) > 0)
				{
					return (TrackPosition.After, -1);
				}
				else
				{
					for (int i = 0; i < ranges.Count; i++)
					{
						var range = ranges[i];
						if (range.Contains(key))
						{
							return (TrackPosition.None, i);
						}
					}
					throw new InvalidOperationException();
				}
			}
			return (TrackPosition.None, -1);
		}
	}
}
