using System;
using System.Collections.Generic;
using System.Text;
using Track.Relation.Tracks;

namespace Track.Relation.Transact
{
	/// <summary>
	/// Словарь отслеживаемых данных
	/// </summary>
	/// <typeparam name="TKey">Тип ключа словаря</typeparam>
	/// <typeparam name="TValue">Тип значения словаря</typeparam>
	/// <typeparam name="TDictionary">Тип словаря</typeparam>
	public class DictionaryObserver<TCommitKey, TKey, TValue, TDictionary> : ObjectTransact<TCommitKey> where TDictionary : IDictionary<TKey, TValue>
	{
		public DictionaryObserver(TDictionary dictionary, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, DispatcherTrack<TCommitKey> dispatcher)
			: base(dispatcher)
		{
			Dictionary = dictionary;
			Track = new DictionaryTrack<TCommitKey, TKey, TValue>(keyComparer, valueComparer);
		}

		/// <summary>
		/// Объект сравнения значений
		/// </summary>
		public IEqualityComparer<TValue> Comparer => Track.Comparer;
		/// <summary>
		/// Отслеживаемый словарь
		/// </summary>
		public TDictionary Dictionary { get; set; }

		/// <summary>
		/// Объект отслеживания изменений
		/// </summary>
		private DictionaryTrack<TCommitKey, TKey, TValue> Track { get; }

		/// <summary>
		/// Зафиксировать данные
		/// </summary>
		/// <param name="indices">Фиксируемые индексы</param>
		public void Commit(IEnumerable<TKey> indices)
		{
			using (new LocalTransaction<TCommitKey>(DispatcherTrack))
			{
				Track.Commit(Dictionary, DispatcherTrack.Transaction, indices);
			}
		}

		protected override void CommitData()
		{
			Track.Commit(Dictionary, DispatcherTrack.Transaction);
		}

		protected override void OffsetData(TCommitKey key)
		{
			Track.Offset(Dictionary, key);
		}

		protected override void RevertData()
		{
			Track.Revert(Dictionary);
		}
	}
}
