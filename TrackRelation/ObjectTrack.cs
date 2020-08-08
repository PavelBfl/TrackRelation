using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	/// <summary>
	/// Базовый объект трансакции
	/// </summary>
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
		/// <summary>
		/// Диспетчер отслеживания
		/// </summary>
		public DispatcherTrack DispatcherTrack { get; }
	}
}
