using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Track.Relation.Tracks;

namespace Track.Relation.Transact
{
	public class ListObserver<TItem, TList> : ObjectTransact where TList : IList<TItem>
	{
		public IEqualityComparer<TItem> Comparer { get; }
		public TList List { get; set; }

		private ListTrack<TItem> Committer { get; }

		public void Commit(IEnumerable<int> indices)
		{
			using (new LocalTransaction(DispatcherTrack))
			{
				Committer.Commit(List, DispatcherTrack.Transaction, indices);
			}
		}

		protected override void CommitData()
		{
			Committer.Commit(List, DispatcherTrack.Transaction);
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
