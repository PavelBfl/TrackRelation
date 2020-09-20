using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	public class TrackRelationOperationException : InvalidOperationException
	{
		public TrackRelationOperationException()
		{

		}
		public TrackRelationOperationException(string message)
			: base(message)
		{

		}
	}
}
