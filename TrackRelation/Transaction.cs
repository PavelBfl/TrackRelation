using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	/// <summary>
	/// Транзакция
	/// </summary>
	public abstract class Transaction : IDisposable
	{
		internal Transaction()
		{

		}

		/// <summary>
		/// Ключ транзакции
		/// </summary>
		public abstract int Key { get; }

		/// <summary>
		/// Завершить работу транзакции
		/// </summary>
		public abstract void Dispose();
	}
}
