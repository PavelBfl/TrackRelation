using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Test.Transact
{
	public class TestBase
	{
		private const string NOT_EMPTY = "abc";
		private static IEnumerable<IEnumerable<object>> CheckData { get; } = new IEnumerable<object>[]
		{
			new object[] { 0, -1, 1, int.MinValue, int.MaxValue },
			new object[]
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
			},
			new[]
			{
				null,
				string.Empty,
				NOT_EMPTY,
			}
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
