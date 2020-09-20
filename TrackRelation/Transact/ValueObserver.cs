using System;
using System.Collections.Generic;
using System.Text;
using Track.Relation.Tracks;

namespace Track.Relation.Transact
{
	/// <summary>
	/// Наблюдатель за изменениями значения
	/// </summary>
	/// <typeparam name="TKey">Тип ключа фиксации</typeparam>
	/// <typeparam name="TValue">Тип наблюдаемого значения</typeparam>
	public class ValueObserver<TKey, TValue> : ObjectTransact<TKey>
	{
		public ValueObserver(IValueAccess<TValue> valueAccess = null, IEqualityComparer<TValue> comparer = null)
		{
			ValueAccess = valueAccess;
			Track = new Track<TKey, TValue>(comparer);
		}

		/// <summary>
		/// Обект доступа к отслеживаемым значениям
		/// </summary>
		public IValueAccess<TValue> ValueAccess { get; set; }

		/// <summary>
		/// Объект сравнения данных
		/// </summary>
		public IEqualityComparer<TValue> Comparer => Track.Comparer;
		private Track<TKey, TValue> Track { get; }

		public override void Commit(Transaction<TKey> transaction)
		{
			Track.SetValue(ValueAccess.GetValue(), transaction);
		}
		public override void Revert()
		{
			if (Track.TryGetLastValue(out var result))
			{
				ValueAccess.SetValue(result);
			}
		}
		public override void Offset(TKey key)
		{
			if (Track.TryGetValue(key, out var result))
			{
				ValueAccess.SetValue(result);
			}
		}
		public override void Clear(TKey begin, TKey end)
		{
			Track.Clear(begin, end);
		}
	}
}
