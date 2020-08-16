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
	public class ListObserver<TItem, TList> : ObjectTransact where TList : IList<TItem>
	{
		public ListObserver(TList list, IEqualityComparer<TItem> comparer)
		{
			List = list;
			Track = new ListTrack<TItem>(comparer);
		}
		public ListObserver(TList list)
			: this(list, default)
		{

		}
		public ListObserver(IEqualityComparer<TItem> comparer)
			: this(default, comparer)
		{

		}
		public ListObserver()
			: this(default, default)
		{

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
		private ListTrack<TItem> Track { get; }

		/// <summary>
		/// Зафиксировать данные
		/// </summary>
		/// <param name="indices">Фиксируемые индексы</param>
		public void Commit(IEnumerable<int> indices)
		{
			using (new LocalTransaction(DispatcherTrack))
			{
				Track.Commit(List, DispatcherTrack.Transaction, indices);
			}
		}

		protected override void CommitData()
		{
			Track.Commit(List, DispatcherTrack.Transaction);
		}

		protected override void OffsetData(int key)
		{
			Track.Offset(List, key);
		}

		protected override void RevertData()
		{
			Track.Revert(List);
		}
	}
}
