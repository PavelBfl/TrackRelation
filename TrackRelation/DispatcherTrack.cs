using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	/// <summary>
	/// Диспетчер отслеживания данных
	/// </summary>
	public class DispatcherTrack<T>
	{
		
		public DispatcherTrack(ICommitKeyProvider<T> commitKeyProvider)
		{
			CommitKeyProvider = commitKeyProvider ?? throw new ArgumentNullException(nameof(commitKeyProvider));
		}

		/// <summary>
		/// Поставщик лючей фиксации
		/// </summary>
		public ICommitKeyProvider<T> CommitKeyProvider { get; }
		/// <summary>
		/// Последний ключ фиксации данных
		/// </summary>
		public T LastCommitKey { get; private set; } = default;

		/// <summary>
		/// Сформировать новый ключ фиксации
		/// </summary>
		/// <returns>Ключ фиксации</returns>
		private T CreateCommitKey()
		{
			var lastCommitKey = new Comparable<T>(LastCommitKey);
			if (!lastCommitKey.IsDefault())
			{
				var newCommitKey = CommitKeyProvider.CreateKey();
				if (lastCommitKey >= newCommitKey)
				{
					throw new InvalidOperationException();
				}
				LastCommitKey = newCommitKey;
			}
			else
			{
				LastCommitKey = CommitKeyProvider.CreateKey();
			}
			return LastCommitKey;
		}

		/// <summary>
		/// Текущая транзакция
		/// </summary>
		public Transaction<T> Transaction { get; private set; }

		/// <summary>
		/// Запустить транзакцию
		/// </summary>
		/// <returns>Транзакция</returns>
		public Transaction<T> BeginCommit()
		{
			if (!(Transaction is null))
			{
				throw new InvalidOperationException();
			}
			return new LocalTransaction(this);
		}


		private class LocalTransaction : Transaction<T>
		{
			public LocalTransaction(DispatcherTrack<T> dispatcher)
			{
				Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
				Dispatcher.Transaction = this;
			}

			public DispatcherTrack<T> Dispatcher { get; }

			public override T Key
			{
				get
				{
					if (new Comparable<T>(key).IsDefault())
					{
						key = Dispatcher.CreateCommitKey();
					}
					return key;
				}
			}
			private T key = default;

			public override void Dispose()
			{
				Dispatcher.Transaction = null;
			}
		}
	}
}
