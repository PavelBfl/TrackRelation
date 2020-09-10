using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Tracks
{
	/// <summary>
	/// Диапазон существования значения
	/// </summary>
	/// <typeparam name="TValue">Тип значения</typeparam>
	struct RangeTrack<TKey, TValue> : IComparable<TKey>, ICommit<TKey, TValue>
	{
		public RangeTrack(TKey begin, TValue value)
			: this(begin, default, value)
		{
			
		}
		public RangeTrack(TKey begin, TKey end, TValue value)
		{
			var beginComparable = new Comparable<TKey>(begin);
			if (beginComparable.IsDefault())
			{
				throw new InvalidOperationException();
			}
			var endComparable = new Comparable<TKey>(end);
			if (!endComparable.IsDefault() && beginComparable >= endComparable)
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

		public TKey Key => Begin;
		public TValue Value { get; }


		/// <summary>
		/// Проверка вхождения ключа в диапазон
		/// </summary>
		/// <param name="key">Провеяемый ключ</param>
		/// <returns>true если ключ входит в диапазон, иначе false</returns>
		public bool Contains(TKey key)
		{
			return CompareTo(key) == 0;
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

		/// <summary>
		/// Сравнить значение ключа фиксации
		/// </summary>
		/// <param name="other">Ключ фиксации</param>
		/// <returns>Значение меньше 0 если ключ меньше начала указаного диапазоно, значение больше 0 если ключ больше значения завершения диапазона, значение равно 0 если ключ входит в диапазон</returns>
		public int CompareTo(TKey other)
		{
			var begin = new Comparable<TKey>(Begin);
			var otherComparable = new Comparable<TKey>(other);
			if (begin > otherComparable)
			{
				return -1;
			}
			else
			{
				var end = new Comparable<TKey>(End);
				if (end.IsDefault() || otherComparable < end)
				{
					return 0;
				}
				else
				{
					return 1;
				}
			}
		}
	}
}
