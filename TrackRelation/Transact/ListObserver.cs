using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Transact
{
	public class ListObserver<TItem, TList> : ObjectTransact where TList : IList<TItem>
	{
		public IEqualityComparer<TItem> Comparer { get; }
		public TList List { get; set; }

		private ListCommitter<TItem> Committer { get; }

		public void Commit(IEnumerable<int> indices)
		{
			using (new LocalTransaction(DispatcherTrack))
			{
				Committer.Commit(List, indices);
			}
		}

		protected override void CommitData()
		{
			Committer.Commit(List);
		}

		protected override void OffsetData(int key)
		{
			Committer.Offset(List, key);
		}

		protected override void RevertData()
		{
			Committer.Revert(List);
		}
	}
}
