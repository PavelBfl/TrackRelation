using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	/// <summary>
	/// Локальная транзакция
	/// </summary>
	class LocalTransaction<TKey> : IDisposable
	{
		public LocalTransaction(DispatcherTrack<TKey> dispatcherTrack)
		{
			DispatcherTrack = dispatcherTrack ?? throw new ArgumentNullException(nameof(dispatcherTrack));

			if (DispatcherTrack.Transaction is null)
			{
				Transaction = DispatcherTrack.BeginCommit();
			}
		}

		private DispatcherTrack<TKey> DispatcherTrack { get; }
		private Transaction<TKey> Transaction { get; }

		public void Dispose()
		{
			Transaction?.Dispose();
		}
	}
}
