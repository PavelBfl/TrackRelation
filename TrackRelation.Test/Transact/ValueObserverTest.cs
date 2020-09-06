using System;
using System.Collections.Generic;
using System.Text;
using Track.Relation;
using Track.Relation.Transact;
using Xunit;

namespace Track.Relation.Test.Transact
{
	public class ValueObserverTest : TestBase
	{
		[Fact]
		public void Constructor_Empty_DefaultComparer()
		{
			var track = new ValueObserver<object, object>();
			Assert.Equal(track.Comparer, EqualityComparer<object>.Default);
		}
		[Fact]
		public void Constructor_SetValueAccess_CustomValueAcces()
		{
			var valueAccess = new ValueAccess<object>(new MockTest<object>());
			var track = new ValueObserver<object, object>(valueAccess: valueAccess);
			Assert.Equal(track.ValueAccess, valueAccess);
		}
		[Fact]
		public void Constructor_SetEqualityComparer_CustomEqualityComparer()
		{
			var comparer = new EqualityComparerMock<object>();
			var track = new ValueObserver<object, object>(comparer: comparer);
			Assert.Equal(track.Comparer, comparer);
		}
		[Theory]
		[MemberData(nameof(ChangeData))]
		public void Value_Revert_InitValue<T>(T initValue, T newValue)
		{
			var commitKeyProvider = new CommitKeyProvider();
			var mockTest = new MockTest<T>()
			{
				PropertyTest = initValue,
			};
			var valueAccess = new ValueAccess<T>(mockTest);
			var track = new ValueObserver<int?, T>(valueAccess);
			track.Commit(commitKeyProvider);
			mockTest.PropertyTest = newValue;
			track.Revert();
			Assert.Equal(mockTest.PropertyTest, initValue);
		}
		[Theory]
		[MemberData(nameof(ChangeData))]
		public void Value_Offset_InitValue<T>(T initValue, T newValue)
		{
			var commitKeyProvider = new CommitKeyProvider();
			var mockTest = new MockTest<T>()
			{
				PropertyTest = initValue,
			};
			var valueAccess = new ValueAccess<T>(mockTest);
			var track = new ValueObserver<int?, T>(valueAccess);
			track.Commit(commitKeyProvider);
			mockTest.PropertyTest = newValue;
			track.Offset(commitKeyProvider.CurrentKey);
			Assert.Equal(mockTest.PropertyTest, initValue);
		}
		[Fact]
		public void Value_WithoutValueAccess_NullReferenceException()
		{
			var track = new ValueObserver<int?, object>();
			Assert.Throws<NullReferenceException>(() => track.Commit(new CommitKeyProvider()));
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
