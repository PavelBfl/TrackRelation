using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	public interface ICommitKeyProvider<T>
	{
		T CreateKey();
	}
}
