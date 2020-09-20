using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	static class ComparableExtension
	{
		public static Comparable<T> AsComparable<T>(this T value)
		{
			return new Comparable<T>(value);
		}
	}
}
