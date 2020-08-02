using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Transact
{
	public abstract class ObjectTransact : ObjectTrack
	{
		public ObjectTransact()
		{

		}
		public ObjectTransact(DispatcherTrack dispatcherTrack)
			: base(dispatcherTrack)
		{

		}

		public void Commit()
		{
			using (new AutoKeyBatch(DispatcherTrack))
			{
				CommitData();
			}
		}
		protected abstract void CommitData();
		public void Revert()
		{
			ThrowIfCommitedEnable();
			RevertData();
		}
		protected abstract void RevertData();
		public void Offset(int key)
		{
			ThrowIfCommitedEnable();
			OffsetData(key);
		}
		protected abstract void OffsetData(int key);

		protected void ThrowIfCommitedEnable()
		{
			if (!(DispatcherTrack.KeyBatch is null))
			{
				throw new InvalidOperationException();
			}
		}
		protected void ThrowIfCommitedDisable()
		{
			if (DispatcherTrack.KeyBatch is null)
			{
				throw new InvalidOperationException();
			}
		}

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
