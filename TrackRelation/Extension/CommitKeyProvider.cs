using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	class CommitKeyProvider : ICommitKeyProvider<int?>
	{
		private int CurrentKey { get; set; }

		public int? CreateKey()
		{
			return CurrentKey++;
		}
	}
}
