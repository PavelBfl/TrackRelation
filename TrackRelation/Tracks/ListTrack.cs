using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Tracks
{
	/// <summary>
	/// Трекер листа
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ListTrack<T>
	{
		public ListTrack()
			: this(null)
		{

		}
		public ListTrack(IEqualityComparer<T> comparer)
		{
			Comparer = comparer ?? EqualityComparer<T>.Default;
		}

		/// <summary>
		/// Объект сравнения данных
		/// </summary>
		public IEqualityComparer<T> Comparer { get; }

		/// <summary>
		/// Базовый элемент отслеживания данных
		/// </summary>
		private List<Track<T>> Track { get; } = new List<Track<T>>();

		/// <summary>
		/// Зафиксировать данные
		/// </summary>
		/// <param name="list">Список данных</param>
		/// <param name="transaction">Транзакция</param>
		/// <param name="indices">Фиксируемые индексы, если установлено null фиксируются все индексы</param>
		public void Commit(IList<T> list, Transaction transaction, IEnumerable<int> indices = null)
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
						Track.Add(new Track<T>(Comparer));
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
		public void Offset(IList<T> list, int key)
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
		public void Revert(IList<T> list)
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
