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
			SortedList<int, string> l = new SortedList<int, string>();

			l.Add(1, "");
			l.Add(10, "");

			var index = l.IndexOfKey(5);
		}
	}
}
