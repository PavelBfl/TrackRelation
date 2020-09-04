using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	/// <summary>
	/// Базовый объект трансакции
	/// </summary>
	public class ObjectTrack<TKey>
	{
		public ObjectTrack(DispatcherTrack<TKey> dispatcherTrack)
		{
			DispatcherTrack = dispatcherTrack ?? throw new ArgumentNullException(nameof(dispatcherTrack));
		}
		/// <summary>
		/// Диспетчер отслеживания
		/// </summary>
		public DispatcherTrack<TKey> DispatcherTrack { get; }
	}
}
