using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Test
{
	public class TestBase
	{
		public static IEnumerable<int> SpecialInt { get; } = new[] { 0, -1, 1, int.MinValue, int.MaxValue };
		public static IEnumerable<double> SpecialDouble { get; } = new[]
		{
			0.0,
			1.0,
			-1.0,
			double.NaN,
			double.MaxValue,
			double.MinValue,
			double.Epsilon,
			double.PositiveInfinity,
			double.NegativeInfinity,
		};
		public static IEnumerable<string> SpecialString { get; } = new[] { null, string.Empty, NOT_EMPTY, };
		private const string NOT_EMPTY = "abc";

		public static IEnumerable<IEnumerable<int>> UniqueCollectionsInt { get; }

		private static IEnumerable<IEnumerable<object>> CheckData { get; } = new IEnumerable<object>[]
		{
			SpecialInt.Cast<object>(),
			SpecialDouble.Cast<object>(),
			SpecialString,
		};
		protected static IEnumerable<object[]> Flat(IEnumerable<IEnumerable<object>> checkData)
		{
			return checkData
				.SelectMany(item => item)
				.Select(item => new object[] { item })
				.ToArray();
		}
		protected static IEnumerable<object[]> Changed(IEnumerable<IEnumerable<object>> checkData)
		{
			return checkData
				.SelectMany(group => group.SelectMany(item => group.Select(groupItem => new object[] { groupItem, item })))
				.ToArray();
		}

		public static IEnumerable<object[]> InitData { get; } = Flat(CheckData);
		public static IEnumerable<object[]> ChangeData { get; } = Changed(CheckData);
	}
}
