using System;
using System.Collections.Generic;
using System.Text;
using Track.Relation.Committers;

namespace Track.Relation.Transact
{
	public class DictionaryObserver<TKey, TValue, TDictionary> : ObjectTransact where TDictionary : IDictionary<TKey, TValue>
	{
		public IEqualityComparer<TValue> Comparer { get; }
		public TDictionary Dictionary { get; set; }

		private DictionaryCommitter<TKey, TValue> Committer { get; }

		public void Commit(IEnumerable<TKey> indices)
		{
			using (new LocalTransaction(DispatcherTrack))
			{
				Committer.Commit(Dictionary, indices);
			}
		}

		protected override void CommitData()
		{
			Committer.Commit(Dictionary);
		}

		protected override void OffsetData(int key)
		{
			Committer.Offset(Dictionary, key);
		}

		protected override void RevertData()
		{
			Committer.Revert(Dictionary);
		}
	}
}
