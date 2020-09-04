using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Tracks
{
	/// <summary>
	/// Трекер листа
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	public class ListTrack<TKey, TValue>
	{
		public ListTrack()
			: this(null)
		{

		}
		public ListTrack(IEqualityComparer<TValue> comparer)
		{
			Comparer = comparer ?? EqualityComparer<TValue>.Default;
		}

		/// <summary>
		/// Объект сравнения данных
		/// </summary>
		public IEqualityComparer<TValue> Comparer { get; }

		/// <summary>
		/// Базовый элемент отслеживания данных
		/// </summary>
		private List<Track<TKey, TValue>> Track { get; } = new List<Track<TKey, TValue>>();

		/// <summary>
		/// Зафиксировать данные
		/// </summary>
		/// <param name="list">Список данных</param>
		/// <param name="transaction">Транзакция</param>
		/// <param name="indices">Фиксируемые индексы, если установлено null фиксируются все индексы</param>
		public void Commit(IList<TValue> list, Transaction<TKey> transaction, IEnumerable<int> indices = null)
		{
			if (list is null)
			{
				throw new ArgumentNullException(nameof(list));
			}
			if (transaction is null)
			{
				throw new ArgumentNullException(nameof(transaction));
			}

			indices = indices ?? Enumerable.Range(0, Math.Max(Track.Count, list.Count));
			foreach (var index in indices)
			{
				if (index < list.Count)
				{
					for (int i = Track.Count; i < list.Count; i++)
					{
						Track.Add(new Track<TKey, TValue>(Comparer));
					}
					Track[index].SetValue(list[index], transaction);
				}
				else
				{
					if (index < list.Count)
					{
						Track[index].Close(transaction);
					}
				}

			} 
		}
		/// <summary>
		/// Получить данные указаной ревизии
		/// </summary>
		/// <param name="list">Контейнер данных</param>
		/// <param name="key">Ключ ревизии</param>
		public void Offset(IList<TValue> list, TKey key)
		{
			if (list is null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			list.Clear();
			foreach (var keyTrack in Track)
			{
				if (keyTrack.TryGetValue(key, out var item))
				{
					list.Add(item);
				}
				else
				{
					return;
				}
			}
		}
		/// <summary>
		/// Отменить изменения до последней ревизии
		/// </summary>
		/// <param name="list">Контейнер данных</param>
		public void Revert(IList<TValue> list)
		{
			if (list is null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			list.Clear();
			foreach (var keyTrack in Track)
			{
				if (keyTrack.TryGetLastValue(out var item))
				{
					list.Add(item);
				}
				else
				{
					return;
				}
			}
		}
	}
}
