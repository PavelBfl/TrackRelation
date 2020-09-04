using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	public class DispatcherTrack
	{
		public DispatcherTrack<int?> Default { get; } = new DispatcherTrack<int?>(new CommitKeyProvider());
	}
}
