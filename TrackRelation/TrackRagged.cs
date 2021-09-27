using System;
using System.Collections.Generic;
using System.Linq;

namespace Track.Relation
{
	public class TrackRagged<TKey, TValue>
	{
		public TrackRagged(IParameters<TKey, TValue> parameters)
		{
			Parameters = parameters ?? new Parameters<TKey, TValue>();
		}

		public IParameters<TKey, TValue> Parameters { get; }

		private List<ISpace> Spaces { get; } = new List<ISpace>();

		public void Add(TKey key, TValue value)
		{
			if (Spaces.Any())
			{
				var lastSpace = Spaces.Last();
				if (Parameters.KeyComparer.Compare(key, lastSpace.Key) <= 0)
				{
					throw new InvalidOperationException();
				}

				switch (lastSpace)
				{
					case Track<TKey, TValue> track:
						track.SetValue(key, value);
						break;
					case EmptySpace _:
						Spaces.Add(new TrackSpace(new Track<TKey, TValue>(key, value, Parameters)));
						break;
					default: throw new InvalidOperationException();
				}
			}
			else
			{
				Spaces.Add(new TrackSpace(new Track<TKey, TValue>(key, value, Parameters)));
			}
		}
		public void Close(TKey key)
		{
			if (Spaces.Any())
			{
				var lastSpace = Spaces.Last();
				if (Parameters.KeyComparer.Compare(key, lastSpace.Key) <= 0)
				{
					throw new InvalidOperationException();
				}

				switch (lastSpace)
				{
					case Track<TKey, TValue> _:
						Spaces.Add(new EmptySpace(key));
						break;
					case EmptySpace _:
						break;
					default: throw new InvalidOperationException();
				}
			}
		}

		private interface ISpace
		{
			TKey Key { get; }
		}
		private class TrackSpace : ISpace
		{
			public TrackSpace(Track<TKey, TValue> track)
			{
				Track = track ?? throw new ArgumentNullException(nameof(track));
			}

			public Track<TKey, TValue> Track { get; }
			public TKey Key => Track.Commits.First().Key;
		}
		private class EmptySpace : ISpace
		{
			public EmptySpace(TKey key)
			{
				Key = key;
			}

			public TKey Key { get; }
		}
	}
}
