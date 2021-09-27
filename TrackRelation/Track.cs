using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace Track.Relation
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
		public Track(TKey key, TValue value)
			: this(key, value, null)
		{
			SetValue(key, value);
		}
		public Track(TKey key, TValue value, IParameters<TKey, TValue> parameters)
			: this(parameters)
		{
			SetValue(key, value);
		}
		public Track(IParameters<TKey, TValue> parameters)
		{
			Parameters = parameters ?? new Parameters<TKey, TValue>();
		}

		/// <summary>
		/// Список диапазонов изменения значений
		/// </summary>
		public IEnumerable<KeyValuePair<TKey, TValue>> Commits => commits;
		private readonly List<KeyValuePair<TKey, TValue>> commits = new List<KeyValuePair<TKey, TValue>>();

		/// <summary>
		/// Объект сравнения значений
		/// </summary>
		public IParameters<TKey, TValue> Parameters { get; }

		/// <summary>
		/// Назначить новое значение
		/// </summary>
		/// <param name="value">Новое значение</param>
		/// <param name="transaction">Транзакция</param>
		/// <returns>true если новое значение было сохранено, иначе false</returns>
		public bool SetValue(TKey key, TValue value)
		{
			if (commits.Count == 0)
			{
				commits.Add(new KeyValuePair<TKey, TValue>(key, Parameters.ValueCloner.Clone(value)));
				return true;
			}
			else
			{
				var lastCommit = commits[commits.Count - 1];
				if (Parameters.KeyComparer.Compare(lastCommit.Key, key) >= 0)
				{
					throw new InvalidOperationException();
				}

				if (!Parameters.ValueComparer.Equals(lastCommit.Value, value))
				{
					commits.Add(new KeyValuePair<TKey, TValue>(key, Parameters.ValueCloner.Clone(value)));
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Получить значение соответствующее ключу
		/// </summary>
		/// <param name="key">Ключ значения</param>
		/// <param name="result">Результирующее значение</param>
		/// <returns>true если значение удалось найти, наче false</returns>
		public bool TryGetValue(TKey key, out TValue result)
		{
			var index = IndexOf(key);
			if (index >= 0)
			{
				result = commits[index].Value;
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}
		/// <summary>
		/// Найти индекс ключа
		/// </summary>
		/// <param name="key">Искомый ключ</param>
		/// <returns>Индекс позиции ключа</returns>
		private int IndexOf(TKey key) => TrackExtension.BinsrySearch(commits, key, x => x.Key, Parameters.KeyComparer);


		/// <summary>
		/// Отчистить сохранённые данные
		/// </summary>
		/// <param name="begin">Ключ фиксации с которого удаляются данные, если значение является default то отчиска происходит с начала</param>
		/// <param name="end">Ключ фиксации до которого удаляются данные, если значение является default то отчистка производится до конца</param>
		public void Clear(TKey begin, TKey end)
		{
			var beginIndex = IndexOf(begin);
			var endIndex = IndexOf(end);

			commits.RemoveRange(beginIndex, endIndex - beginIndex + 1);
		}
	}
}
