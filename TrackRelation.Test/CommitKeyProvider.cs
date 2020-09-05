using System;
using System.Collections.Generic;
using System.Text;
using Track.Relation;

namespace TrackRelation.Test
{
	class CommitKeyProvider : ICommitKeyProvider<int?>
	{
		public int CurrentKey { get; private set; } = -1;

		public int? CreateKey()
		{
			return ++CurrentKey;
		}
	}
}
