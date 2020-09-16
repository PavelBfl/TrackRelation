using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Track.Relation.Test
{
	public class TransactionTest
	{
		[Fact]
		public void Constructor_SetCustomCommitKeyProvider_CustomCommitKeyProvider()
		{
			var commitKeyProvider = new CommitKeyProvider();
			using (var transaction = new Transaction<int?>(commitKeyProvider))
			{
				Assert.Equal(commitKeyProvider, transaction.CommitKeyProvider);
			}
		}
		[Fact]
		public void Constructor_SetNullCustomCommitKeyProvider_ArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new Transaction<object>(null));
		}
		[Fact]
		public void Key_GenerateKey_NewKey()
		{
			var commitKeyProvider = new CommitKeyProvider();
			using (var transaction = new Transaction<int?>(commitKeyProvider))
			{
				var key = transaction.Key;
				Assert.Equal(commitKeyProvider.CurrentKey, key);
			}
		}
		[Fact]
		public void Key_Dispose_ObjectDisposedException()
		{
			var commitKeyProvider = new CommitKeyProvider();
			var transaction = new Transaction<int?>(commitKeyProvider);
			transaction.Dispose();
			Assert.Throws<ObjectDisposedException>(() => transaction.Key);
		}
	}
}
