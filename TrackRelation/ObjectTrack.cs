using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	public class ObjectTrack
	{
		public ObjectTrack()
		{

		}

		protected KeyProvider KeyProvider { get; } = new KeyProvider();
	}
}
