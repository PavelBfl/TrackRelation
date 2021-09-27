using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Track.Relation.Test
{
	public class TestBase
	{
		[Fact]
		public void Test()
		{
			var array = new[] { 1, 19 };
			var result = TrackExtension.BinsrySearch(array, 10, x => x, Comparer<int>.Default);
		}
	}
}
