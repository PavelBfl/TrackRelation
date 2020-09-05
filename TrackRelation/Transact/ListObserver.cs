using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Track.Relation.Tracks;

namespace Track.Relation.Transact
{
	/// <summary>
	/// Наблюдатель за коллекцией
	/// </summary>
	/// <typeparam name="TItem">Тип элемента коллекции</typeparam>
	/// <typeparam name="TList">Тип коллекции</typeparam>
	public class ListObserver<TKey, TItem, TList> : ObjectTransact<TKey> where TList : IList<TItem>
	{
		public ListObserver(TList list, IEqualityComparer<TItem> comparer)
		{
			List = list;
			Track = new ListTrack<TKey, TItem>(comparer);
		}

		/// <summary>
		/// Объект сравнения данных
		/// </summary>
		public IEqualityComparer<TItem> Comparer => Track.Comparer;
		/// <summary>
		/// Наблюдаемый объект
		/// </summary>
		public TList List { get; set; }

		/// <summary>
		/// Трекер изменений
		/// </summary>
		private ListTrack<TKey, TItem> Track { get; }

		/// <summary>
		/// Зафиксировать данные
		/// </summary>
		/// <param name="indices">Фиксируемые индексы</param>
		/// <param name="transaction">Транзакция</param>
		public void Commit(IEnumerable<int> indices, Transaction<TKey> transaction)
		{
			Track.Commit(List, transaction, indices);
		}

		public override void Commit(Transaction<TKey> transaction)
		{
			Track.Commit(List, transaction);
		}

		public override void Offset(TKey key)
		{
			Track.Offset(List, key);
		}

		public override void Revert()
		{
			Track.Revert(List);
		}
	}
}
