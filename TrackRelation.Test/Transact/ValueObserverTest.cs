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
		[Fact]
		public void Constructor_SetValueAccess_CustomValueAcces()
		{
			var valueAccess = new ValueAccess<object>(new MockTest<object>());
			var track = new ValueObserver<object>(valueAccess: valueAccess);
			Assert.Equal(track.ValueAccess, valueAccess);
		}
		[Fact]
		public void Constructor_SetDispatcher_CustomDispatcher()
		{
			var dispatcher = new DispatcherTrack();
			var track = new ValueObserver<object>(dispatcher: dispatcher);
			Assert.Equal(track.DispatcherTrack, dispatcher);
		}
		[Fact]
		public void Constructor_SetEqualityComparer_CustomEqualityComparer()
		{
			var comparer = new EqualityComparerMock<object>();
			var track = new ValueObserver<object>(comparer: comparer);
			Assert.Equal(track.Comparer, comparer);
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
