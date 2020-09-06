using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Track.Relation.Test
{
	class EqualityComparerMock<T> : IEqualityComparer<T>
	{
		public bool Equals([AllowNull] T x, [AllowNull] T y)
		{
			return EqualityComparer<T>.Default.Equals(x, y);
		}

		public int GetHashCode([DisallowNull] T obj)
		{
			return EqualityComparer<T>.Default.GetHashCode(obj);
		}
	}
}
