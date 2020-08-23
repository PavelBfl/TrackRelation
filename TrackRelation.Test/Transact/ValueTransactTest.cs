using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Track.Relation;
using Track.Relation.Transact;
using Xunit;

namespace Track.Relation.Test.Transact
{
	public class ValueTransactTest
	{
		[Fact]
		public void Constructor_Empty_DispatcherDefault()
		{
			var track = new ValueTransact<object>();
			Assert.Equal(track.DispatcherTrack, DispatcherTrack.Default);
		}
		[Fact]
		public void Constructor_Empty_TrackUndefined()
		{
			var track = new ValueTransact<object>();
			Assert.True(track.IsUndefined);
		}
		[Fact]
		public void Constructor_Empty_GetValueInvalidOperationException()
		{
			var track = new ValueTransact<object>();
			Assert.Throws<InvalidOperationException>(() => { var _ = track.Value; });
		}
		[Fact]
		public void Constructor_Dispatcher_SetDispatcher()
		{
			var dispatcher = new DispatcherTrack();
			var track = new ValueTransact<object>(dispatcher);
			Assert.Equal(track.DispatcherTrack, dispatcher);
		}
		[Fact]
		public void Constructor_Dispatcher_TrackUndefined()
		{
			var dispatcher = new DispatcherTrack();
			var track = new ValueTransact<object>(dispatcher);
			Assert.True(track.IsUndefined);
		}
		[Fact]
		public void Constructor_Dispatcher_GetValueInvalidOperationException()
		{
			var dispatcher = new DispatcherTrack();
			var track = new ValueTransact<object>(dispatcher);
			Assert.Throws<InvalidOperationException>(() => { var _ = track.Value; });
		}

		[Theory]
		[MemberData(nameof(InitData))]
		public void Constructor_Value_DispatcherDefault<T>(T initValue)
		{
			var track = new ValueTransact<T>(initValue);
			Assert.Equal(track.DispatcherTrack, DispatcherTrack.Default);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Constructor_Value_TrackDefined<T>(T initValue)
		{
			var track = new ValueTransact<T>(initValue);
			Assert.False(track.IsUndefined);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Constructor_Value_InitValue<T>(T initValue)
		{
			var track = new ValueTransact<T>(initValue);
			Assert.Equal(track.Value, initValue);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Close_DefinedData_Undefined<T>(T initValue)
		{
			var track = new ValueTransact<T>(initValue, dispatcher: new DispatcherTrack());
			track.Close();
			Assert.True(track.IsUndefined);
		}
		[Fact]
		public void Close_UndefinedData_Undefined()
		{
			var track = new ValueTransact<object>(new DispatcherTrack());
			track.Close();
			Assert.True(track.IsUndefined);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Value_NewValue_NewValue<T>(T newValue)
		{
			var track = new ValueTransact<T>(dispatcher: new DispatcherTrack());
			track.Value = newValue;
			Assert.Equal(track.Value, newValue);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Value_NewValue_ValueDefined<T>(T newValue)
		{
			var track = new ValueTransact<T>(dispatcher: new DispatcherTrack());
			track.Value = newValue;
			Assert.False(track.IsUndefined);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Value_NewValue_InvalidOperationException<T>(T newValue)
		{
			var track = new ValueTransact<T>(dispatcher: new DispatcherTrack());
			using var transact = track.DispatcherTrack.BeginCommit();
			Assert.Throws<InvalidOperationException>(() => track.Value = newValue);
		}
		[Theory]
		[MemberData(nameof(ChangeData))]
		public void Value_Revert_InitValue<T>(T initValue, T newValue)
		{
			var track = new ValueTransact<T>(initValue, dispatcher: new DispatcherTrack());
			track.Commit();
			track.Value = newValue;
			track.Revert();
			Assert.Equal(track.Value, initValue);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Value_Revert_Undefined<T>(T newValue)
		{
			var track = new ValueTransact<T>(dispatcher: new DispatcherTrack());
			track.Commit();
			track.Value = newValue;
			track.Revert();
			Assert.True(track.IsUndefined);
		}

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

		public static IEnumerable<object[]> InitData { get; } = CheckData
			.SelectMany(item => item)
			.Select(item => new object[] { item })
			.ToArray();
		public static IEnumerable<object[]> ChangeData { get; } = CheckData
			.SelectMany(group => group.SelectMany(item => group.Select(groupItem => new object[] { groupItem, item })))
			.ToArray();
	}
}
