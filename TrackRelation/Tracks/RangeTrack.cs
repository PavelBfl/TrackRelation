using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Tracks
{
	/// <summary>
	/// Диапазон существования значения
	/// </summary>
	/// <typeparam name="T">Тип значения</typeparam>
	struct RangeTrack<T>
	{
		private RangeTrack(int begin, int? end, T value)
		{
			if (begin >= end)
			{
				throw new ArgumentOutOfRangeException(nameof(end));
			}

			Begin = begin;
			End = end;
			Value = value;
		}
		public RangeTrack(int begin, T value)
			: this(begin, null, value)
		{
			
		}
		public RangeTrack(int begin, int end, T value)
			: this(begin, new int?(end), value)
		{

		}
		/// <summary>
		/// Начало существования значения
		/// </summary>
		public int Begin { get; }
		/// <summary>
		/// Конец существования значения, если null то существование значения не закрыто
		/// </summary>
		public int? End { get; }
		/// <summary>
		/// Отслеживаемое значение
		/// </summary>
		public T Value { get; }

		/// <summary>
		/// Проверка вхождения ключа в диапазон
		/// </summary>
		/// <param name="key">Провеяемый ключ</param>
		/// <returns>true если ключ входит в диапазон, иначе false</returns>
		public bool Contains(int key)
		{
			return Begin <= key && key < (End ?? int.MaxValue);
		}
		/// <summary>
		/// Создать закрытую копию диапазона
		/// </summary>
		/// <param name="end">Ключ на котором необходимо закрыть значение</param>
		/// <returns>Закрытый диапазон</returns>
		public RangeTrack<T> Close(int end)
		{
			return new RangeTrack<T>(Begin, end, Value);
		}
	}
}
