using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Transact
{
	/// <summary>
	/// Объект потдержки транзакций
	/// </summary>
	public abstract class ObjectTransact<TKey>
	{
		/// <summary>
		/// Зафиксировать данные
		/// </summary>
		/// <param name="commitKeyProvider">Поставщик ключей фиксации</param>
		public void Commit(ICommitKeyProvider<TKey> commitKeyProvider)
		{
			using (var transaction = new Transaction<TKey>(commitKeyProvider))
			{
				Commit(transaction);
			}
		}
		/// <summary>
		/// Зафиксировать данные
		/// </summary>
		/// <param name="transaction">Транзакция</param>
		public abstract void Commit(Transaction<TKey> transaction);
		/// <summary>
		/// Отменить изменения
		/// </summary>
		public abstract void Revert();
		/// <summary>
		/// Сместить данные до ревизии
		/// </summary>
		/// <param name="key">Ключ ревизии</param>
		public abstract void Offset(TKey key);
	}
}
