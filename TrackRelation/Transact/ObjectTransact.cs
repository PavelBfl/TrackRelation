using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Transact
{
	/// <summary>
	/// Объект потдержки транзакций
	/// </summary>
	public abstract class ObjectTransact<TKey> : ObjectTrack<TKey>
	{
		public ObjectTransact(DispatcherTrack<TKey> dispatcherTrack)
			: base(dispatcherTrack)
		{

		}

		/// <summary>
		/// Зафиксировать данные
		/// </summary>
		public void Commit()
		{
			using (new LocalTransaction<TKey>(DispatcherTrack))
			{
				CommitData();
			}
		}
		/// <summary>
		/// Зафиксировать внутриние данные
		/// </summary>
		protected abstract void CommitData();
		/// <summary>
		/// Отменить изменения
		/// </summary>
		public void Revert()
		{
			ThrowIfCommitedEnable();
			RevertData();
		}
		/// <summary>
		/// Отменить внутриние изменения
		/// </summary>
		protected abstract void RevertData();
		/// <summary>
		/// Сместить данные до ревизии
		/// </summary>
		/// <param name="key">Ключ ревизии</param>
		public void Offset(TKey key)
		{
			ThrowIfCommitedEnable();
			OffsetData(key);
		}
		/// <summary>
		/// Сместить внутриние данные до ревизии
		/// </summary>
		/// <param name="key">Ключ ревизии</param>
		protected abstract void OffsetData(TKey key);

		/// <summary>
		/// Вызвать исключение если производится фиксация данных
		/// </summary>
		protected void ThrowIfCommitedEnable()
		{
			if (!(DispatcherTrack.Transaction is null))
			{
				throw new InvalidOperationException();
			}
		}
		/// <summary>
		/// Вызвать исключение если фиксация не активна
		/// </summary>
		protected void ThrowIfCommitedDisable()
		{
			if (DispatcherTrack.Transaction is null)
			{
				throw new InvalidOperationException();
			}
		}
	}
}
