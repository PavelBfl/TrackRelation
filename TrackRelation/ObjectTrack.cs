using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	public class ObjectTrack
	{
		public ObjectTrack()
		{
			DispatcherTrack = DispatcherTrack.Default;
		}
		public ObjectTrack(DispatcherTrack dispatcherTrack)
		{
			DispatcherTrack = dispatcherTrack ?? throw new ArgumentNullException(nameof(dispatcherTrack));
		}

		public DispatcherTrack DispatcherTrack { get; }
	}
}
