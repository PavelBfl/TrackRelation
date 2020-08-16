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
	public class DictionaryObserver<TKey, TValue, TDictionary> : ObjectTransact where TDictionary : IDictionary<TKey, TValue>
	{
		public DictionaryObserver(TDictionary dictionary, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
		{
			Dictionary = dictionary;
			Track = new DictionaryTrack<TKey, TValue>(keyComparer, valueComparer);
		}
		public DictionaryObserver(TDictionary dictionary, IEqualityComparer<TKey> keyComparer)
			: this(dictionary, keyComparer, default)
		{

		}
		public DictionaryObserver(TDictionary dictionary)
			: this(dictionary, default, default)
		{

		}
		public DictionaryObserver(IEqualityComparer<TValue> valueComparer)
			: this(default, default, valueComparer)
		{

		}
		public DictionaryObserver()
			: this(default, default, default)
		{

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
		private DictionaryTrack<TKey, TValue> Track { get; }

		/// <summary>
		/// Зафиксировать данные
		/// </summary>
		/// <param name="indices">Фиксируемые индексы</param>
		public void Commit(IEnumerable<TKey> indices)
		{
			using (new LocalTransaction(DispatcherTrack))
			{
				Track.Commit(Dictionary, DispatcherTrack.Transaction, indices);
			}
		}

		protected override void CommitData()
		{
			Track.Commit(Dictionary, DispatcherTrack.Transaction);
		}

		protected override void OffsetData(int key)
		{
			Track.Offset(Dictionary, key);
		}

		protected override void RevertData()
		{
			Track.Revert(Dictionary);
		}
	}
}
