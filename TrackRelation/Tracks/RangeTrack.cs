using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Tracks
{
	/// <summary>
	/// Диапазон существования значения
	/// </summary>
	/// <typeparam name="TValue">Тип значения</typeparam>
	struct RangeTrack<TKey, TValue>
	{
		public RangeTrack(TKey begin, TValue value)
		{
			Begin = begin;
			End = default;
			Value = value;
		}
		public RangeTrack(TKey begin, TKey end, TValue value)
		{
			if (Comparer<TKey>.Default.Compare(begin, end) >= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(end));
			}
			Begin = begin;
			End = end;
			Value = value;
		}
		/// <summary>
		/// Начало существования значения
		/// </summary>
		public TKey Begin { get; }
		/// <summary>
		/// Конец существования значения, если null то существование значения не закрыто
		/// </summary>
		public TKey End { get; }
		/// <summary>
		/// Отслеживаемое значение
		/// </summary>
		public TValue Value { get; }

		/// <summary>
		/// Проверка вхождения ключа в диапазон
		/// </summary>
		/// <param name="key">Провеяемый ключ</param>
		/// <returns>true если ключ входит в диапазон, иначе false</returns>
		public bool Contains(TKey key)
		{
			var begin = new Comparable<TKey>(Begin);
			var end = new Comparable<TKey>(End);
			if (end.IsDefault())
			{
				return begin <= key;
			}
			else
			{
				return begin <= key && key < end;
			}
		}
		/// <summary>
		/// Создать закрытую копию диапазона
		/// </summary>
		/// <param name="end">Ключ на котором необходимо закрыть значение</param>
		/// <returns>Закрытый диапазон</returns>
		public RangeTrack<TKey, TValue> Close(TKey end)
		{
			return new RangeTrack<TKey, TValue>(Begin, end, Value);
		}
	}
}
