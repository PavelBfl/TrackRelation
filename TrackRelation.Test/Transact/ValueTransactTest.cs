using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Track.Relation;
using Track.Relation.Transact;
using TrackRelation.Test;
using TrackRelation.Test.Transact;
using Xunit;

namespace Track.Relation.Test.Transact
{
	public class ValueTransactTest : ValueTestBase
	{
		[Fact]
		public void Constructor_Empty_TrackUndefined()
		{
			var track = new ValueTransact<object, object>();
			Assert.True(track.IsUndefined);
		}
		[Fact]
		public void Constructor_Empty_GetValueInvalidOperationException()
		{
			var track = new ValueTransact<object, object>();
			Assert.Throws<InvalidOperationException>(() => { var _ = track.Value; });
		}

		[Theory]
		[MemberData(nameof(InitData))]
		public void Constructor_Value_TrackDefined<T>(T initValue)
		{
			var track = new ValueTransact<object, T>(initValue);
			Assert.False(track.IsUndefined);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Constructor_Value_InitValue<T>(T initValue)
		{
			var track = new ValueTransact<object, T>(initValue);
			Assert.Equal(track.Value, initValue);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Close_DefinedData_Undefined<T>(T initValue)
		{
			var track = new ValueTransact<object, T>(initValue);
			track.Close();
			Assert.True(track.IsUndefined);
		}
		[Fact]
		public void Close_UndefinedData_Undefined()
		{
			var track = new ValueTransact<object, object>();
			track.Close();
			Assert.True(track.IsUndefined);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Value_NewValue_NewValue<T>(T newValue)
		{
			var track = new ValueTransact<object, T>();
			track.Value = newValue;
			Assert.Equal(track.Value, newValue);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Value_NewValue_ValueDefined<T>(T newValue)
		{
			var track = new ValueTransact<object, T>();
			track.Value = newValue;
			Assert.False(track.IsUndefined);
		}
		[Theory]
		[MemberData(nameof(ChangeData))]
		public void Value_Revert_InitValue<T>(T initValue, T newValue)
		{
			var track = new ValueTransact<int?, T>(initValue);
			track.Commit(new CommitKeyProvider());
			track.Value = newValue;
			track.Revert();
			Assert.Equal(track.Value, initValue);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Value_Revert_Undefined<T>(T newValue)
		{
			var track = new ValueTransact<int?, T>();
			track.Commit(new CommitKeyProvider());
			track.Value = newValue;
			track.Revert();
			Assert.True(track.IsUndefined);
		}
	}
}
