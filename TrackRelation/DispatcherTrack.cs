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

		public KeyBatch KeyBatch { get; private set; }

		public KeyBatch BeginCommit()
		{
			if (!(KeyBatch is null))
			{
				throw new InvalidOperationException();
			}
			return new LocalKeyBatch(this);
		}

		private class LocalKeyBatch : KeyBatch
		{
			public LocalKeyBatch(DispatcherTrack keyProvider)
			{
				KeyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
				KeyProvider.KeyBatch = this;
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
				KeyProvider.KeyBatch = null;
			}
		}
	}
}
