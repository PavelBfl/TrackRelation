using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
		private const string BEGIN_OVER_END_MESSAGE = "Ключ начала больше ключа завершения поиска";
		private const string INVALID_FIND_MESSAGE = "Сбой поиска данных";
		private const string UNKNOWN_POSITION_MESSAGE = "Неизвестное значение позиции поиска";
		private const string POSITION_BEGIN_OVER_END_MESSAGE = "Позиция начала отчистки больше позиции завершения";
		private const string INVALID_FIND_POSITION_MESSAGE = "Не удалось определить позицию ключа на шкале отслеживания";

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

			var position = FindPosition(key);
			if (position.State == PositionState.Begin || position.State == PositionState.Range)
			{
				result = ranges[position.Index].Value;
				return true;
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
		/// <param name="begin">Ключ фиксации с которого удаляются данные, если значение является default то отчиска происходит с начала</param>
		/// <param name="end">Ключ фиксации до которого удаляются данные, если значение является default то отчистка производится до конца</param>
		public void Clear(TKey begin, TKey end)
		{
			var beginComparable = begin.AsComparable();
			var endComparable = end.AsComparable();
			if (!beginComparable.IsDefault() && !endComparable.IsDefault() && beginComparable > endComparable)
			{
				throw new TrackRelationOperationException(BEGIN_OVER_END_MESSAGE);
			}

			var beginPosition = beginComparable.IsDefault() && ranges.Any() ? Position.Before : FindPosition(begin);
			var endPosition = endComparable.IsDefault() && ranges.Any() ? Position.After : FindPosition(end);

			switch (beginPosition.State)
			{
				case PositionState.None:
					if (endPosition.State != PositionState.None)
					{
						throw new TrackRelationOperationException(INVALID_FIND_MESSAGE);
					}
					break;
				case PositionState.Before:
					switch (endPosition.State)
					{
						case PositionState.Before:
							// Оба индекса находятся до начала истории отслеживания, отчистку производить не требуется
							break;
						case PositionState.After:
							ranges.Clear();
							break;
						case PositionState.Skip:
						case PositionState.Begin:
						case PositionState.Range:
							RemoveRange(0, endPosition.Index);
							break;
						case PositionState.None: throw new TrackRelationOperationException(INVALID_FIND_MESSAGE);
						default: throw new TrackRelationOperationException(UNKNOWN_POSITION_MESSAGE);
					}
					break;
				case PositionState.After:
					if (endPosition.State != PositionState.After)
					{
						throw new TrackRelationOperationException(POSITION_BEGIN_OVER_END_MESSAGE);
					}
					break;
				case PositionState.Range:
				case PositionState.Skip:
					switch (endPosition.State)
					{
						case PositionState.After:
							RemoveRange(beginPosition.NextIndex, ranges.Count - 1);
							break;
						case PositionState.Skip:
						case PositionState.Begin:
						case PositionState.Range:
							RemoveRange(beginPosition.NextIndex, endPosition.Index);
							break;
						case PositionState.Before: throw new TrackRelationOperationException(POSITION_BEGIN_OVER_END_MESSAGE);
						case PositionState.None: throw new TrackRelationOperationException(INVALID_FIND_MESSAGE);
						default: throw new TrackRelationOperationException(UNKNOWN_POSITION_MESSAGE);
					}
					break;
				case PositionState.Begin:
					switch (endPosition.State)
					{
						case PositionState.After:
							RemoveRange(beginPosition.Index, ranges.Count - 1);
							break;
						case PositionState.Skip:
						case PositionState.Begin:
						case PositionState.Range:
							RemoveRange(beginPosition.Index, endPosition.Index);
							break;
						case PositionState.Before: throw new TrackRelationOperationException(POSITION_BEGIN_OVER_END_MESSAGE);
						case PositionState.None: throw new TrackRelationOperationException(INVALID_FIND_MESSAGE);
						default: throw new TrackRelationOperationException(UNKNOWN_POSITION_MESSAGE);
					}
					break;
				default: throw new TrackRelationOperationException(UNKNOWN_POSITION_MESSAGE);
			}
		}
		/// <summary>
		/// Удалить данные коллекцию данных
		/// </summary>
		/// <param name="beginIndex">Индекс с которого начать удаление включительно</param>
		/// <param name="endIndex">Индекс до которого необходимо удалять включительно</param>
		private void RemoveRange(int beginIndex, int endIndex)
		{
			ranges.RemoveRange(beginIndex, endIndex - beginIndex + 1);
		}

		/// <summary>
		/// Найти индекс диапазона по идентификатору фиксации
		/// </summary>
		/// <param name="key">Идентификатор фиксации</param>
		/// <returns>Индекс найденого элемента, -1 если ключ меньше первого элемента, range.Count если ключ больше последнего элемента</returns>
		private Position FindPosition(TKey key)
		{
			if (ranges.Any())
			{
				var keyComparable = key.AsComparable();
				if (keyComparable < ranges.First().Begin.AsComparable())
				{
					return Position.Before;
				}
				else if (ranges.Last().Begin.AsComparable() < keyComparable)
				{
					return Position.After;
				}
				else
				{
					for (int i = 0; i < ranges.Count; i++)
					{
						var range = ranges[i];
						var compare = range.CompareTo(key);
						if (compare == 0)
						{
							if (keyComparable == range.Begin.AsComparable())
							{
								return Position.Begin(i);
							}
							else
							{
								return Position.Range(i);
							}
						}
						else if (compare < 0)
						{
							return Position.Skip(i - 1);
						}
					}
					throw new TrackRelationOperationException(INVALID_FIND_POSITION_MESSAGE);
				}
			}
			return Position.None;
		}

		/// <summary>
		/// Состьояние позиции поиска
		/// </summary>
		private enum PositionState
		{
			/// <summary>
			/// Не удплось определить позицию
			/// </summary>
			None,
			/// <summary>
			/// Позиция находится до первого индекса отслеживаемых данных
			/// </summary>
			Before,
			/// <summary>
			/// Позиция находится после последнего индекса отслеживаемых данных
			/// </summary>
			After,
			/// <summary>
			/// Позиция находится в промежутке между двумя точками отслеживания
			/// </summary>
			Skip,
			/// <summary>
			/// Позицея находится в индексе списка отслеживания данных
			/// </summary>
			Range,
			/// <summary>
			/// Позиция находится в начале времени значения
			/// </summary>
			Begin,
		}
		/// <summary>
		/// Позиция поиска
		/// </summary>
		private struct Position
		{
			public static Position None { get; } = new Position(PositionState.None, default);
			public static Position Before { get; } = new Position(PositionState.Before, default);
			public static Position After { get; } = new Position(PositionState.After, default);
			public static Position Skip(int prevIndex) => new Position(PositionState.Skip, prevIndex);
			public static Position Range(int index) => new Position(PositionState.Range, index);
			public static Position Begin(int index) => new Position(PositionState.Begin, index);

			private Position(PositionState state, int index)
			{
				State = state;
				Index = index >= 0 ? index : throw new IndexOutOfRangeException();
			}

			/// <summary>
			/// Состояние позиции
			/// </summary>
			public PositionState State { get; }
			/// <summary>
			/// Значение индекса данных, актуален при <see cref="State"/> равном <see cref="PositionState.Skip"/> или <see cref="PositionState.Index"/>
			/// </summary>
			public int Index { get; }
			/// <summary>
			/// Предшествующий индекс, актуален при <see cref="State"/> равном <see cref="PositionState.Skip"/>
			/// </summary>
			public int PrevIndex => Index;
			/// <summary>
			/// Следующий индекс, актуален при <see cref="State"/> равном <see cref="PositionState.Skip"/>
			/// </summary>
			public int NextIndex => Index + 1;
		}
	}
}
