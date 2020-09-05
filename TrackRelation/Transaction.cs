using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	/// <summary>
	/// Транзакция
	/// </summary>
	public class Transaction<T> : IDisposable
	{
		public Transaction(ICommitKeyProvider<T> commitKeyProvider)
		{
			CommitKeyProvider = commitKeyProvider ?? throw new ArgumentNullException(nameof(commitKeyProvider));
		}

		/// <summary>
		/// Поставщик ключей фиксации
		/// </summary>
		public ICommitKeyProvider<T> CommitKeyProvider { get; }

		/// <summary>
		/// Ключ транзакции
		/// </summary>
		public T Key
		{
			get
			{
				if (new Comparable<T>(key).IsDefault())
				{
					key = CommitKeyProvider.CreateKey();
				}
				return key;
			}
		}
		private T key = default;

		/// <summary>
		/// Завершить работу транзакции
		/// </summary>
		public void Dispose()
		{
			
		}
	}
}
