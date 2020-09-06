using System;
using System.Collections.Generic;
using System.Text;
using Track.Relation.Transact;
using Xunit;

namespace Track.Relation.Test.Transact
{
	public class ListObserverTest : TestBase
	{
		private static IEnumerable<IEnumerable<object>> CheckDataList { get; } = new object[][]
		{
			new object[]
			{
				new int[] { },
				new int[] { -1, 0, 1 },
				new int[] { 0, 1, 2, 3, 4, 5, 6 ,7 ,8 ,9 },
			},
			new object[]
			{
				new double[] { },
				new double[] { -1, 0, 1 },
				new double[] { 0, 1, 2, 3, 4, 5, 6 ,7 ,8 ,9 },
			},
			new object[]
			{
				new string[] { },
				new string[] { null, string.Empty, "a", "b", "c" },
				new string[] { "0", "1", "2", "3", "4", "5", "6" ,"7" ,"8" ,"9" },
			}
		};


		public static IEnumerable<object[]> InitDataList { get; } = Flat(CheckDataList);
		public static IEnumerable<object[]> ChangeDataList { get; } = Changed(CheckDataList);

		[Fact]
		public void Constructor_Empty_ListDefault()
		{
			var listObserver = new ListObserver<object, object, List<object>>();
			Assert.Equal(default, listObserver.List);
		}
		[Fact]
		public void Constructor_SetList_CustomList()
		{
			var list = new List<object>();
			var listObserver = new ListObserver<object, object, List<object>>(list: list);
			Assert.Equal(list, listObserver.List);
		}
		[Fact]
		public void Constructor_Empty_ComparerDefault()
		{
			var listObserver = new ListObserver<object, object, List<object>>();
			Assert.Equal(EqualityComparer<object>.Default, listObserver.Comparer);
		}
		[Fact]
		public void Constructor_SetComparator_CustomComparer()
		{
			var comparer = new EqualityComparerMock<object>();
			var listObserver = new ListObserver<object, object, List<object>>(comparer: comparer);
			Assert.Equal(comparer, listObserver.Comparer);
		}
		[Fact]
		public void Commit_WithoutList_NullReferenceException()
		{
			var listObserver = new ListObserver<int?, object, List<object>>();
			Assert.Throws<ArgumentNullException>(() => listObserver.Commit(new CommitKeyProvider()));
		}

		[Theory]
		[MemberData(nameof(InitDataList))]
		public void Revert_RevertData_EmptyList<T>(T[] initData)
		{
			var listObserver = new ListObserver<int?, T, List<T>>(list: new List<T>());
			listObserver.Commit(new CommitKeyProvider());
			listObserver.List.AddRange(initData);
			listObserver.Revert();
			Assert.Empty(listObserver.List);
		}
		[Theory]
		[MemberData(nameof(ChangeDataList))]
		public void Revert_RevertData_InitData<T>(T[] initData, T[] newData)
		{
			var listObserver = new ListObserver<int?, T, List<T>>(list: new List<T>(initData));
			listObserver.Commit(new CommitKeyProvider());
			listObserver.List.AddRange(newData);
			listObserver.Revert();
			Assert.Equal(initData, listObserver.List);
		}
		[Theory]
		[MemberData(nameof(InitDataList))]
		public void Offset_OffsetData_EmptyList<T>(T[] initData)
		{
			var commitKeyProvider = new CommitKeyProvider();
			var listObserver = new ListObserver<int?, T, List<T>>(list: new List<T>());
			listObserver.Commit(commitKeyProvider);
			listObserver.List.AddRange(initData);
			listObserver.Offset(commitKeyProvider.CurrentKey);
			Assert.Empty(listObserver.List);
		}
		[Theory]
		[MemberData(nameof(ChangeDataList))]
		public void Offset_OffsetData_InitData<T>(T[] initData, T[] newData)
		{
			var commitKeyProvider = new CommitKeyProvider();
			var listObserver = new ListObserver<int?, T, List<T>>(list: new List<T>(initData));
			listObserver.Commit(commitKeyProvider);
			listObserver.List.AddRange(newData);
			listObserver.Offset(commitKeyProvider.CurrentKey);
			Assert.Equal(initData, listObserver.List);
		}
	}
}
