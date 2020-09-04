using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Track.Relation
{
	struct Comparable<T>
	{
		public Comparable(T value)
		{
			Value = value;
		}

		public T Value { get; }

		public bool IsDefault()
		{
			return this == default(T);
		}

		private static bool Equals(T first, T second)
		{
			return EqualityComparer<T>.Default.Equals(first, second);
		}
		private static int Compare(T first, T second)
		{
			return Comparer<T>.Default.Compare(first, second);
		}

		public static bool operator ==(Comparable<T> first, Comparable<T> second)
		{
			return Equals(first.Value, second.Value);
		}
		public static bool operator !=(Comparable<T> first, Comparable<T> second)
		{
			return !(first == second);
		}
		public static bool operator ==(T first, Comparable<T> second)
		{
			return Equals(first, second.Value);
		}
		public static bool operator !=(T first, Comparable<T> second)
		{
			return !(first == second);
		}
		public static bool operator ==(Comparable<T> first, T second)
		{
			return Equals(first.Value, second);
		}
		public static bool operator !=(Comparable<T> first, T second)
		{
			return !(first == second);
		}
		public static bool operator >(Comparable<T> first, Comparable<T> second)
		{
			return Compare(first.Value, second.Value) > 0;
		}
		public static bool operator <(Comparable<T> first, Comparable<T> second)
		{
			return Compare(first.Value, second.Value) < 0;
		}
		public static bool operator >(T first, Comparable<T> second)
		{
			return Compare(first, second.Value) > 0;
		}
		public static bool operator <(T first, Comparable<T> second)
		{
			return Compare(first, second.Value) < 0;
		}
		public static bool operator >(Comparable<T> first, T second)
		{
			return Compare(first.Value, second) > 0;
		}
		public static bool operator <(Comparable<T> first, T second)
		{
			return Compare(first.Value, second) < 0;
		}
		public static bool operator >=(Comparable<T> first, Comparable<T> second)
		{
			return Compare(first.Value, second.Value) >= 0;
		}
		public static bool operator <=(Comparable<T> first, Comparable<T> second)
		{
			return Compare(first.Value, second.Value) <= 0;
		}
		public static bool operator >=(T first, Comparable<T> second)
		{
			return Compare(first, second.Value) >= 0;
		}
		public static bool operator <=(T first, Comparable<T> second)
		{
			return Compare(first, second.Value) <= 0;
		}
		public static bool operator >=(Comparable<T> first, T second)
		{
			return Compare(first.Value, second) >= 0;
		}
		public static bool operator <=(Comparable<T> first, T second)
		{
			return Compare(first.Value, second) <= 0;
		}

		public static explicit operator Comparable<T>(T value)
		{
			return new Comparable<T>(value);
		}
		public static explicit operator T(Comparable<T> value)
		{
			return value.Value;
		}

		public override bool Equals(object obj)
		{
			if (obj is Comparable<T> comparable)
			{
				return this == comparable;
			}
			else if (obj is T value)
			{
				return this == value;
			}
			return false;
		}
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}
