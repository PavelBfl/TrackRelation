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
	class Track<TValue>
	{
		public Track()
			: this(null)
		{
			
		}
		public Track(TValue value, Transaction keyBatch)
			: this(value, null, keyBatch)
		{
			SetValue(value, keyBatch);
		}
		public Track(TValue value, IEqualityComparer<TValue> comparer, Transaction keyBatch)
			: this(comparer)
		{
			SetValue(value, keyBatch);
		}
		public Track(IEqualityComparer<TValue> comparer)
		{
			Comparer = comparer ?? EqualityComparer<TValue>.Default;
		}

		/// <summary>
		/// Список диапазонов изменения значений
		/// </summary>
		private readonly List<RangeTrack<TValue>> track = new List<RangeTrack<TValue>>();
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
		public bool SetValue(TValue value, Transaction transaction)
		{
			if (!TryGetLastValue(out var lastValue) || !Comparer.Equals(value, lastValue))
			{
				Close(transaction);
				track.Add(new RangeTrack<TValue>(transaction.Key, value));
				return true;
			}
			return false;
		}
		/// <summary>
		/// Закрыть существование значения
		/// </summary>
		/// <param name="transaction">Транзакция</param>
		public void Close(Transaction transaction)
		{
			var lastIndex = track.Count - 1;
			if (lastIndex >= 0)
			{
				var lastRange = track[lastIndex];
				if (lastRange.End is null)
				{
					track[lastIndex] = lastRange.Close(transaction.Key);
				}
			}
		}

		/// <summary>
		/// Получить значение соответствующее ключу
		/// </summary>
		/// <param name="key">Ключ значения</param>
		/// <param name="result">Результирующее значение</param>
		/// <returns>true если значение удалось найти, наче false</returns>
		public bool TryGetValue(int key, out TValue result)
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
			var lastIndex = track.Count - 1;
			if (lastIndex >= 0)
			{
				var lastRange = track[lastIndex];
				if (lastRange.End is null)
				{
					result = lastRange.Value;
					return true;
				}
			}

			result = default;
			return false;
		}
	}
}
