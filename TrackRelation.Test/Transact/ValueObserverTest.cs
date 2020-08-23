using System;
using System.Collections.Generic;
using System.Text;
using Track.Relation;
using Track.Relation.Transact;
using Xunit;

namespace TrackRelation.Test.Transact
{
	public class ValueObserverTest
	{
		[Fact]
		public void Constructor_Empty_DefaultDispatcher()
		{
			var track = new ValueObserver<object>();
			Assert.Equal(track.DispatcherTrack, DispatcherTrack.Default);
		}
		[Fact]
		public void Constructor_Empty_DefaultComparer()
		{
			var track = new ValueObserver<object>();
			Assert.Equal(track.Comparer, EqualityComparer<object>.Default);
		}
		


		private class MockTest<T>
		{
			public T PropertyTest { get; set; }
		}

		private class ValueAccess<T> : IValueAccess<T>
		{
			public ValueAccess(MockTest<T> owner)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			}

			public MockTest<T> Owner { get; }

			public T GetValue()
			{
				return Owner.PropertyTest;
			}

			public void SetValue(T value)
			{
				Owner.PropertyTest = value;
			}
		}
	}
}
