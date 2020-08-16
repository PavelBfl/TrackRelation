using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	/// <summary>
	/// Диспетчер отслеживания данных
	/// </summary>
	public class DispatcherTrack
	{
		/// <summary>
		/// Диспетчер по умолчанию
		/// </summary>
		public static DispatcherTrack Default { get; } = new DispatcherTrack();

		/// <summary>
		/// Текущий индекс ревизии
		/// </summary>
		public int CurrentIndex { get; private set; } = -1;

		/// <summary>
		/// Получить новое значение идентификатора ревизии
		/// </summary>
		/// <returns>Идентификатор ревизии</returns>
		public int GetNewKey()
		{
			return ++CurrentIndex;
		}

		/// <summary>
		/// Текущая транзакция
		/// </summary>
		public Transaction Transaction { get; private set; }

		/// <summary>
		/// Запустить транзакцию
		/// </summary>
		/// <returns>Транзакция</returns>
		public Transaction BeginCommit()
		{
			if (!(Transaction is null))
			{
				throw new InvalidOperationException();
			}
			return new LocalTransaction(this);
		}


		private class LocalTransaction : Transaction
		{
			public LocalTransaction(DispatcherTrack keyProvider)
			{
				KeyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
				KeyProvider.Transaction = this;
			}

			public DispatcherTrack KeyProvider { get; }

			public override int Key
			{
				get
				{
					key = key ?? KeyProvider.GetNewKey();
					return key.Value;
				}
			}
			private int? key = null;

			public override void Dispose()
			{
				KeyProvider.Transaction = null;
			}
		}
	}
}
