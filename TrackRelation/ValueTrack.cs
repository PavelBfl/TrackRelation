using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Track.Relation
{
	class ValueTrack<TValue>
	{
		public ValueTrack()
			: this(EqualityComparer<TValue>.Default)
		{

		}
		public ValueTrack(IEqualityComparer<TValue> equalityComparer)
		{
			EqualityComparer = equalityComparer;
		}

		private IEqualityComparer<TValue> EqualityComparer { get; }
		private KeyProvider KeyProvider { get; } = new KeyProvider();
		private List<Pair> Values { get; } = new List<Pair>();

		public TValue Value
		{
			get => value;
			set
			{
				if (!EqualityComparer.Equals(Value, value))
				{
					this.value = value;
					Values.Add(new Pair(KeyProvider.GetNewKey(), Value));
				}
			}
		}
		private TValue value;

		public void OffsetToKey(int key)
		{
			if (TryGetPair(key, out var pair))
			{
				value = pair.Value;
			}
		}
		private bool TryGetPair(int key, out Pair result)
		{
			foreach (var pair in Values.AsEnumerable().Reverse())
			{
				if (pair.Key <= key)
				{
					result = pair;
					return true;
				}
			}

			result = default;
			return false;
		}

		private struct Pair
		{
			public Pair(int key, TValue value)
			{
				Key = key;
				Value = value;
			}

			public int Key { get; }
			public TValue Value { get; }
		}
	}
}
