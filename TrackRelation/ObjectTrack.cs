using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	public abstract class ObjectTrack
	{
		public ObjectTrack()
		{

		}

		protected KeyProvider KeyProvider { get; } = new KeyProvider();

		public void Offset(int key)
		{
			if (key < 0 || KeyProvider.CurrentIndex < key)
			{
				throw new ArgumentOutOfRangeException(nameof(key));
			}

			OffsetData(key);
		}

		protected abstract void OffsetData(int key);
	}
}
