using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Track.Relation;
using Track.Relation.Transact;
using Xunit;

namespace Track.Relation.Test.Transact
{
	public abstract class ValueTransactTest<T>
	{
		[Fact]
		public void Constructor_Empty_DispatcherDefault()
		{
			var track = new ValueTransact<T>();
			Assert.Equal(track.DispatcherTrack, DispatcherTrack.Default);
		}
		[Fact]
		public void Constructor_Empty_TrackUndefined()
		{
			var track = new ValueTransact<T>();
			Assert.True(track.IsUndefined);
		}
		[Fact]
		public void Constructor_Empty_GetValueInvalidOperationException()
		{
			var track = new ValueTransact<T>();
			Assert.Throws<InvalidOperationException>(() => { var _ = track.Value; });
		}
		[Fact]
		public void Constructor_Dispatcher_SetDispatcher()
		{
			var dispatcher = new DispatcherTrack();
			var track = new ValueTransact<T>(dispatcher);
			Assert.Equal(track.DispatcherTrack, dispatcher);
		}
		[Fact]
		public void Constructor_Dispatcher_TrackUndefined()
		{
			var dispatcher = new DispatcherTrack();
			var track = new ValueTransact<T>(dispatcher);
			Assert.True(track.IsUndefined);
		}
		[Fact]
		public void Constructor_Dispatcher_GetValueInvalidOperationException()
		{
			var dispatcher = new DispatcherTrack();
			var track = new ValueTransact<T>(dispatcher);
			Assert.Throws<InvalidOperationException>(() => { var _ = track.Value; });
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Constructor_Value_DispatcherDefault(T initValue)
		{
			var track = new ValueTransact<T>(initValue);
			Assert.Equal(track.DispatcherTrack, DispatcherTrack.Default);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Constructor_Value_TrackDefined(T initValue)
		{
			var track = new ValueTransact<T>(initValue);
			Assert.False(track.IsUndefined);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Constructor_Value_InitValue(T initValue)
		{
			var track = new ValueTransact<T>(initValue);
			Assert.Equal(track.Value, initValue);
		}

		[Theory]
		[MemberData(nameof(InitData))]
		public void Close_DefinedData_Undefined(T initValue)
		{
			var track = new ValueTransact<T>(initValue, new DispatcherTrack());
			track.Close();
			Assert.True(track.IsUndefined);
		}
		[Fact]
		public void Close_UndefinedData_Undefined()
		{
			var track = new ValueTransact<T>(new DispatcherTrack());
			track.Close();
			Assert.True(track.IsUndefined);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Value_NewValue_NewValue(T newValue)
		{
			var track = new ValueTransact<T>(new DispatcherTrack());
			track.Value = newValue;
			Assert.Equal(track.Value, newValue);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Value_NewValue_ValueDefined(T newValue)
		{
			var track = new ValueTransact<T>(new DispatcherTrack());
			track.Value = newValue;
			Assert.False(track.IsUndefined);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Value_NewValue_InvalidOperationException(T newValue)
		{
			var track = new ValueTransact<T>(new DispatcherTrack());
			using var transact = track.DispatcherTrack.BeginCommit();
			Assert.Throws<InvalidOperationException>(() => track.Value = newValue);
		}
		[Theory]
		[MemberData(nameof(ChangeData))]
		public void Value_Revert_InitValue(T initValue, T newValue)
		{
			var track = new ValueTransact<T>(initValue, new DispatcherTrack());
			track.Commit();
			track.Value = newValue;
			track.Revert();
			Assert.Equal(track.Value, initValue);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Value_Revert_Undefined(T newValue)
		{
			var track = new ValueTransact<T>(new DispatcherTrack());
			track.Commit();
			track.Value = newValue;
			track.Revert();
			Assert.True(track.IsUndefined);
		}

		private static IEnumerable<object> CheckData { get; } = CreateCheckData();
		private static IEnumerable<object> CreateCheckData()
		{
			switch (Type.GetTypeCode(typeof(T)))
			{
				case TypeCode.Int32: return ValueTransactTestInt.CheckData;
				case TypeCode.Double: return ValueTransactTestDouble.CheckData;
				case TypeCode.String: return ValueTransactTestString.CheckData;
				default: return new object[] { default(T) };
			}
		}

		public static IEnumerable<object[]> InitData { get; } = CheckData.Select(item => new object[] { item }).ToArray();
		public static IEnumerable<object[]> ChangeData { get; } = CheckData.SelectMany(initValue => CheckData.Select(newValue => new object[] { initValue, newValue })).ToArray();
	}

	public class ValueTransactTestInt : ValueTransactTest<int>
	{
		public static IEnumerable<object> CheckData { get; } = new object[] { 0, -1, 1, int.MinValue, int.MaxValue };
	}
	public class ValueTransactTestDouble : ValueTransactTest<double>
	{
		public static IEnumerable<object> CheckData { get; } = new object[]
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
	}
	public class ValueTransactTestString : ValueTransactTest<string>
	{
		private const string NOT_EMPTY = "abc";

		public static IEnumerable<object> CheckData { get; } = new[]
		{
			null,
			string.Empty,
			NOT_EMPTY,
		};
	}
}
