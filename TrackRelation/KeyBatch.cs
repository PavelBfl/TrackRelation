using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	public abstract class KeyBatch : IDisposable
	{
		internal KeyBatch()
		{

		}

		public abstract int Key { get; }

		public abstract void Dispose();
	}
}
