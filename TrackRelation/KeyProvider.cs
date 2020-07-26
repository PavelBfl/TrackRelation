using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	public class KeyProvider
	{
		public int CurrentIndex { get; private set; } = -1;

		public int GetNewKey()
		{
			return ++CurrentIndex;
		}
	}
}
