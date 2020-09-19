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

			var position = FindIndex(key);
			if (position.State == PositionState.Index)
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
			var beginComparable = new Comparable<TKey>(begin);
			var endComparable = new Comparable<TKey>(end);
			if (!beginComparable.IsDefault() && !endComparable.IsDefault() && beginComparable > endComparable)
			{
				throw new InvalidOperationException();
			}

			var beginPosition = beginComparable.IsDefault() ? Position.Before : FindIndex(begin);
			var endPosition = endComparable.IsDefault() ? Position.After : FindIndex(end);

			switch (beginPosition.State)
			{
				case PositionState.None:
					if (endPosition.State != PositionState.None)
					{
						throw new InvalidOperationException();
					}
					break;
				case PositionState.Before:
					switch (endPosition.State)
					{
						case PositionState.Before:
							break;
						case PositionState.After:
							ranges.Clear();
							break;
						case PositionState.Skip:
						case PositionState.Index:
							RemoveRange(0, endPosition.Index);
							break;
						default: throw new InvalidOperationException();
					}
					break;
				case PositionState.After:
					if (endPosition.State != PositionState.After)
					{
						throw new InvalidOperationException();
					}
					break;
				case PositionState.Skip:
					switch (endPosition.State)
					{
						case PositionState.After:
							RemoveRange(beginPosition.NextIndex, ranges.Count - 1);
							break;
						case PositionState.Skip:
						case PositionState.Index:
							RemoveRange(beginPosition.NextIndex, endPosition.Index);
							break;
						default: throw new InvalidOperationException();
					}
					break;
				case PositionState.Index:
					switch (endPosition.State)
					{
						case PositionState.After:
							RemoveRange(beginPosition.Index, ranges.Count - 1);
							break;
						case PositionState.Skip:
						case PositionState.Index:
							RemoveRange(beginPosition.Index, endPosition.Index);
							break;
						default: throw new InvalidOperationException();
					}
					break;
				default: throw new InvalidOperationException();
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
		private Position FindIndex(TKey key)
		{
			if (ranges.Any())
			{
				if (ranges.First().CompareTo(key) < 0)
				{
					return Position.Before;
				}
				else if (ranges.Last().CompareTo(key) > 0)
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
							return Position.AsIndex(i);
						}
						else if (compare < 0)
						{
							return Position.Skip(i - 1);
						}
					}
					throw new InvalidOperationException();
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
			Index,
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
			public static Position AsIndex(int index) => new Position(PositionState.Index, index);

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
