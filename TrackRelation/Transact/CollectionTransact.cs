using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation.Transact
{
	public abstract class CollectionTransact<TKey, TValue> : ObjectTransact
	{
		private IIndicesCollection<TKey, ValueTrack<TValue>> Tracks { get; }
		private IIndicesCollection<TKey, TValue> Items { get; }
		private HashSet<TKey> ChangedKeys { get; } = new HashSet<TKey>();

		public IEqualityComparer<TValue> Comparator { get; }

		protected void ChangeKey(TKey key)
		{
			ChangedKeys.Add(key);
		}

		
		protected override void CommitData()
		{
			if (ChangedKeys.Any())
			{
				foreach (var key in ChangedKeys)
				{
					if (Items.TryGetValue(key, out var item))
					{
						if (Tracks.TryGetValue(key, out var track))
						{
							track.SetValue(item, DispatcherTrack.Transaction);
						}
						else
						{
							Tracks.Add(key, new ValueTrack<TValue>(item, Comparator, DispatcherTrack.Transaction));
						}
					}
					else
					{
						if (Tracks.TryGetValue(key, out var track))
						{
							track.Close(DispatcherTrack.Transaction);
						}
					}
				}
				ChangedKeys.Clear();
			}
		}
	}
}
