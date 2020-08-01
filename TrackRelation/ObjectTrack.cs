using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	public abstract class ObjectTrack
	{
		public ObjectTrack()
		{
			DispatcherTrack = DispatcherTrack.Default;
		}
		public ObjectTrack(DispatcherTrack dispatcherTrack)
		{
			DispatcherTrack = dispatcherTrack ?? throw new ArgumentNullException(nameof(dispatcherTrack));
		}

		protected DispatcherTrack DispatcherTrack { get; }

		public void Commit()
		{
			using (new AutoKeyBatch(DispatcherTrack))
			{
				CommitData();
			}
		}
		protected abstract void CommitData();
		public abstract void Revert();
		public abstract void Offset(int key);

		private class AutoKeyBatch : IDisposable
		{
			public AutoKeyBatch(DispatcherTrack dispatcherTrack)
			{
				DispatcherTrack = dispatcherTrack ?? throw new ArgumentNullException(nameof(dispatcherTrack));

				if (DispatcherTrack.KeyBatch is null)
				{
					LocalKeyBatch = DispatcherTrack.BeginCommit();
				}
			}

			private DispatcherTrack DispatcherTrack { get; }
			private KeyBatch LocalKeyBatch { get; }

			public void Dispose()
			{
				LocalKeyBatch?.Dispose();
			}
		}
	}
}
