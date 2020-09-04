using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	/// <summary>
	/// Транзакция
	/// </summary>
	public abstract class Transaction<T> : IDisposable
	{
		internal Transaction()
		{

		}

		/// <summary>
		/// Ключ транзакции
		/// </summary>
		public abstract T Key { get; }

		/// <summary>
		/// Завершить работу транзакции
		/// </summary>
		public abstract void Dispose();
	}
}
