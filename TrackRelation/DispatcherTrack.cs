using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	public class DispatcherTrack
	{
		public static DispatcherTrack Default { get; } = new DispatcherTrack();

		public int CurrentIndex { get; private set; } = -1;

		public int GetNewKey()
		{
			return ++CurrentIndex;
		}

		public Transaction Transaction { get; private set; }

		public Transaction BeginCommit()
		{
			if (!(Transaction is null))
			{
				throw new InvalidOperationException();
			}
			return new LocalTransaction(this);
		}

		private class LocalTransaction : Transaction
		{
			public LocalTransaction(DispatcherTrack keyProvider)
			{
				KeyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
				KeyProvider.Transaction = this;
			}

			public DispatcherTrack KeyProvider { get; }

			public override int Key
			{
				get
				{
					key = key ?? KeyProvider.GetNewKey();
					return key.Value;
				}
			}
			private int? key = null;

			public override void Dispose()
			{
				KeyProvider.Transaction = null;
			}
		}
	}
}
