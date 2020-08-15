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
			using (new LocalTransaction(DispatcherTrack))
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
			if (!(DispatcherTrack.Transaction is null))
			{
				throw new InvalidOperationException();
			}
		}
		protected void ThrowIfCommitedDisable()
		{
			if (DispatcherTrack.Transaction is null)
			{
				throw new InvalidOperationException();
			}
		}
	}
}
