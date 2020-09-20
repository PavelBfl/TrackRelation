using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Track.Relation.Tracks;
using Xunit;

namespace Track.Relation.Test.Tracks
{
	public class TrackTest : TestBase
	{
		public static IEnumerable<IEnumerable<object>> CheckDataTrack { get; } = new object[][]
		{
			new object[]
			{
				new HashSet<int>(),
				new HashSet<int>() { -1, 0, 1 },
				new HashSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
			},
			new object[]
			{
				new HashSet<double>(),
				new HashSet<double>() { -1, 0, 1 },
				new HashSet<double>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
			},
			new object[]
			{
				new HashSet<string>() { },
				new HashSet<string>() { null, string.Empty, "a", "b", "c" },
				new HashSet<string>() { "0", "1", "2", "3", "4", "5", "6" ,"7" ,"8" ,"9" },
			}
		};
		public static IEnumerable<object[]> InitTrack { get; } = Flat(CheckDataTrack);

		private static Track<TKey, TValue> CreateTrack<TKey, TValue>(TValue value, ICommitKeyProvider<TKey> commitKeyProvider)
		{
			using (var transaction = new Transaction<TKey>(commitKeyProvider))
			{
				return new Track<TKey, TValue>(value, transaction);
			}
		}
		private static Track<TKey, TValue> CreateTrack<TKey, TValue>(IEnumerable<TValue> values, ICommitKeyProvider<TKey> commitKeyProvider)
		{
			var track = new Track<TKey, TValue>();
			foreach (var value in values)
			{
				using var transaction = new Transaction<TKey>(commitKeyProvider);
				track.SetValue(value, transaction);
			}
			return track;
		}

		[Fact]
		public void Constructor_Empty_DefaultComparer()
		{
			var track = new Track<object, object>();
			Assert.Equal(EqualityComparer<object>.Default, track.Comparer);
		}
		[Fact]
		public void Constructor_Empty_CommitsEmpty()
		{
			var track = new Track<object, object>();
			Assert.Empty(track.Commits);
		}
		[Fact]
		public void Constructor_SetComparer_CustomComparer()
		{
			var comparer = new EqualityComparerMock<object>();
			var track = new Track<object, object>(comparer);
			Assert.Equal(comparer, track.Comparer);
		}
		[Fact]
		public void Constructor_SetNullConstructor_DefaultComparer()
		{
			var track = new Track<object, object>(null);
			Assert.Equal(EqualityComparer<object>.Default, track.Comparer);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Constructor_SetValue_InitCommit<T>(T value)
		{
			var commitKeyProvider = new CommitKeyProvider();
			using var transaction = new Transaction<int?>(commitKeyProvider);
			var track = new Track<int?, T>(value, transaction);
			Assert.Single(track.Commits);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Constructor_SetValue_InitValueCommit<T>(T value)
		{
			var commitKeyProvider = new CommitKeyProvider();
			using var transaction = new Transaction<int?>(commitKeyProvider);
			var track = new Track<int?, T>(value, transaction);
			Assert.Equal(value, track.Commits.Single().Value);
		}
		[Fact]
		public void Constructor_SetValue_DefaultComparer()
		{
			var commitKeyProvider = new CommitKeyProvider();
			using var transaction = new Transaction<int?>(commitKeyProvider);
			var track = new Track<int?, object>(null, transaction);
			Assert.Equal(EqualityComparer<object>.Default, track.Comparer);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Constructor_SetValueSetComparer_InitCommit<T>(T value)
		{
			var commitKeyProvider = new CommitKeyProvider();
			using var transaction = new Transaction<int?>(commitKeyProvider);
			var track = new Track<int?, T>(value, new EqualityComparerMock<T>(), transaction);
			Assert.Single(track.Commits);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Constructor_SetValueSetComparer_InitValueCommit<T>(T value)
		{
			var commitKeyProvider = new CommitKeyProvider();
			using var transaction = new Transaction<int?>(commitKeyProvider);
			var track = new Track<int?, T>(value, new EqualityComparerMock<T>(), transaction);
			Assert.Equal(value, track.Commits.Single().Value);
		}
		[Fact]
		public void Constructor_SetValueSetComparer_CustomComparer()
		{
			var commitKeyProvider = new CommitKeyProvider();
			using var transaction = new Transaction<int?>(commitKeyProvider);
			var comparer = new EqualityComparerMock<object>();
			var track = new Track<int?, object>(null, comparer, transaction);
			Assert.Equal(comparer, track.Comparer);
		}
		[Fact]
		public void TryGetLastValue_Empty_UndefinedValue()
		{
			var track = new Track<object, object>();
			Assert.False(track.TryGetLastValue(out var _));
		}
		[Fact]
		public void TryGetLastValue_InitValue_DefinedValue()
		{
			var commitKeyProvider = new CommitKeyProvider();
			using var transaction = new Transaction<int?>(commitKeyProvider);
			var track = new Track<int?, object>(null, transaction);
			Assert.True(track.TryGetLastValue(out var _));
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void TryGetLastValue_InitValue_CustomValue<T>(T value)
		{
			var commitKeyProvider = new CommitKeyProvider();
			using var transaction = new Transaction<int?>(commitKeyProvider);
			var track = new Track<int?, T>(value, transaction);

			track.TryGetLastValue(out var lastValue);
			Assert.Equal(value, lastValue);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void TryGetLastValue_CommitValue_CustomValue<T>(T value)
		{
			var commitKeyProvider = new CommitKeyProvider();
			using var transaction = new Transaction<int?>(commitKeyProvider);
			var track = new Track<int?, T>();
			track.SetValue(value, transaction);

			track.TryGetLastValue(out var lastValue);
			Assert.Equal(value, lastValue);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void SetValue_NewValue_SetSuccess<T>(T value)
		{
			var commitKeyProvider = new CommitKeyProvider();
			using var transaction = new Transaction<int?>(commitKeyProvider);
			var track = new Track<int?, T>();
			var success = track.SetValue(value, transaction);
			Assert.True(success);
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void SetValue_SameValue_SetFail<T>(T value)
		{
			var commitKeyProvider = new CommitKeyProvider();
			var track = CreateTrack(value, commitKeyProvider);

			using (var transaction = new Transaction<int?>(commitKeyProvider))
			{
				var success = track.SetValue(value, transaction);
				Assert.False(success);
			}
		}
		[Theory]
		[MemberData(nameof(ChangeData))]
		public void SetValue_NewValue_SuccessIfChanged<T>(T initValue, T newValue)
		{
			var commitKeyProvider = new CommitKeyProvider();
			var track = CreateTrack(initValue, commitKeyProvider);

			using (var transaction = new Transaction<int?>(commitKeyProvider))
			{
				var success = track.SetValue(newValue, transaction);
				Assert.Equal(!track.Comparer.Equals(initValue, newValue), success);
			}
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void Close_InitValue_LastValueUndefined<T>(T value)
		{
			var commitKeyProvider = new CommitKeyProvider();
			var track = CreateTrack(value, commitKeyProvider);
			
			using (var transaction = new Transaction<int?>(commitKeyProvider))
			{
				track.Close(transaction);
				Assert.False(track.TryGetLastValue(out var _));
			}
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void TryGetValue_ExistsKey_GetSuccess<T>(T value)
		{
			var commitKeyProvider = new CommitKeyProvider();
			var track = CreateTrack(value, commitKeyProvider);

			Assert.True(track.TryGetValue(commitKeyProvider.CurrentKey, out var _));
		}
		[Theory]
		[MemberData(nameof(InitData))]
		public void TryGetValue_ExistsKey_CommitValue<T>(T value)
		{
			var commitKeyProvider = new CommitKeyProvider();
			var track = CreateTrack(value, commitKeyProvider);

			track.TryGetValue(commitKeyProvider.CurrentKey, out var commitValue);
			Assert.Equal(value, commitValue);
		}
		[Fact]
		public void TryGetValue_UndefinedKey_GetFail()
		{
			var track = new Track<object, object>();
			Assert.False(track.TryGetValue(new object(), out var _));
		}
		[Fact]
		public void TryGetValue_DefaultKey_InvalidOperationException()
		{
			var track = new Track<object, object>();
			Assert.Throws<InvalidOperationException>(() => track.TryGetValue(null, out var _));
		}

		[Theory]
		[MemberData(nameof(InitTrack))]
		public void Clear_All_CommitsEmpty<T>(IEnumerable<T> values)
		{
			var track = CreateTrack(values, new CommitKeyProvider());
			track.Clear(default, default);
			Assert.Empty(track.Commits);
		}
		[Theory]
		[MemberData(nameof(InitTrack))]
		public void Clear_FirstHalf_SecondHalf<T>(IEnumerable<T> values)
		{
			var track = CreateTrack(values, new CommitKeyProvider());
			var halfIndex = track.Commits.Count() / 2;
			var halfKey = track.Commits.ElementAtOrDefault(halfIndex)?.Key;
			var expected = track.Commits.Skip(halfIndex + 1).ToArray();
			track.Clear(default, halfKey);

			Assert.Equal(track.Commits, expected);
		}
		[Theory]
		[MemberData(nameof(InitTrack))]
		public void Clear_SecondHalf_FirstHalf<T>(IEnumerable<T> values)
		{
			var track = CreateTrack(values, new CommitKeyProvider());
			var halfIndex = track.Commits.Count() / 2;
			var halfKey = track.Commits.ElementAtOrDefault(halfIndex)?.Key;
			var expected = track.Commits.Take(halfIndex).ToArray();
			track.Clear(halfKey, default);

			Assert.Equal(track.Commits, expected);
		}
		[Theory]
		[MemberData(nameof(InitTrack))]
		public void Clear_Center_WithoutCenter<T>(IEnumerable<T> values)
		{
			var track = CreateTrack(values, new CommitKeyProvider());
			var beginIndex = track.Commits.Count() / 3;
			var endIndex = beginIndex * 2;
			var expected = track.Commits.Take(beginIndex).Concat(track.Commits.Skip(endIndex + 1)).ToArray();
			var beginKey = track.Commits.ElementAtOrDefault(beginIndex)?.Key;
			var endKey = track.Commits.ElementAtOrDefault(endIndex)?.Key;
			track.Clear(beginKey, endKey);

			Assert.Equal(expected, track.Commits, CommitEqualityComparer<int?, T>.Instance);
		}
		[Theory]
		[MemberData(nameof(InitTrack))]
		public void Clear_SingleCenter_WithoutSingleCenter<T>(IEnumerable<T> values)
		{
			var track = CreateTrack(values, new CommitKeyProvider());
			var halfIndex = track.Commits.Count() / 2;
			var expected = track.Commits.Take(halfIndex).Concat(track.Commits.Skip(halfIndex + 1)).ToArray();
			var centerKey = track.Commits.ElementAtOrDefault(halfIndex)?.Key;
			track.Clear(centerKey, centerKey);

			Assert.Equal(expected, track.Commits, CommitEqualityComparer<int?, T>.Instance);
		}

		[Theory]
		[InlineData(null, null)]
		[InlineData(0, null)]
		[InlineData(null, 0)]
		[InlineData(0, 0)]
		[InlineData(0, 1)]
		public void Clear_Empty_CommitsEmpty(int? begin, int? end)
		{
			var track = new Track<int?, object>();
			track.Clear(begin, end);
			Assert.Empty(track.Commits);
		}

		private class CommitEqualityComparer<TKey, TValue> : IEqualityComparer<ICommit<TKey, TValue>>
		{
			public static CommitEqualityComparer<TKey, TValue> Instance { get; } = new CommitEqualityComparer<TKey, TValue>();

			private CommitEqualityComparer()
			{

			}

			public bool Equals([AllowNull] ICommit<TKey, TValue> x, [AllowNull] ICommit<TKey, TValue> y)
			{
				if (ReferenceEquals(x, y))
				{
					return true;
				}
				else if (x is null || y is null)
				{
					return false;
				}
				else
				{
					return EqualityComparer<TKey>.Default.Equals(x.Key, y.Key) && EqualityComparer<TValue>.Default.Equals(x.Value, y.Value);
				}
			}

			public int GetHashCode([DisallowNull] ICommit<TKey, TValue> obj)
			{
				return HashCode.Combine(obj.Key, obj.Value);
			}
		}
	}
}
