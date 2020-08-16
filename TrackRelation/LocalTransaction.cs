using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	/// <summary>
	/// Локальная транзакция
	/// </summary>
	class LocalTransaction : IDisposable
	{
		public LocalTransaction(DispatcherTrack dispatcherTrack)
		{
			DispatcherTrack = dispatcherTrack ?? throw new ArgumentNullException(nameof(dispatcherTrack));

			if (DispatcherTrack.Transaction is null)
			{
				Transaction = DispatcherTrack.BeginCommit();
			}
		}

		private DispatcherTrack DispatcherTrack { get; }
		private Transaction Transaction { get; }

		public void Dispose()
		{
			Transaction?.Dispose();
		}
	}
}
