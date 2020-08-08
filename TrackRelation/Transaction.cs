using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	public abstract class Transaction : IDisposable
	{
		internal Transaction()
		{

		}

		public abstract int Key { get; }

		public abstract void Dispose();
	}
}
