using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	class KeyProvider
	{
		private int CurrentIndex { get; set; } = 0;

		public int GetNewKey()
		{
			return CurrentIndex++;
		}
	}
}
