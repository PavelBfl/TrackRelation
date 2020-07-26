using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Collections
{
	class KeyTrackBuilder
	{
		public KeyTrackBuilder(KeyProvider keyProvider)
		{
			KeyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
		}

		private KeyProvider KeyProvider { get; }
		private int? keyTrack = null;

		public int GetKey()
		{
			keyTrack = keyTrack ?? KeyProvider.GetNewKey();
			return keyTrack.Value;
		}
	}
}
