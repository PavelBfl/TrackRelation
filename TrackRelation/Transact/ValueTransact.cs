using System;
using System.Collections.Generic;
using System.Text;
using Track.Relation.Tracks;

namespace Track.Relation.Transact
{
	/// <summary>
	/// Отслеживаемое значение
	/// </summary>
	/// <typeparam name="TKey">Тип ключа фиксации</typeparam>
	/// <typeparam name="TValue">Тип изменяемого значения</typeparam>
	public class ValueTransact<TKey, TValue> : ObjectTransact<TKey>
	{
		public ValueTransact(TValue value, IEqualityComparer<TValue> comparer = null)
			: this(comparer)
		{
			Value = value;
		}
		public ValueTransact(IEqualityComparer<TValue> comparer = null)
		{
			Track = new Track<TKey, TValue>(comparer);
		}

		/// <summary>
		/// Объект ставнения данных
		/// </summary>
		public IEqualityComparer<TValue> Comparer => Track.Comparer;
		private Track<TKey, TValue> Track { get; }

		/// <summary>
		/// Текущее значение
		/// </summary>
		public TValue Value
		{
			get
			{
				if (IsUndefined)
				{
					throw new InvalidOperationException();
				}
				return value;
			}
			set
			{
				this.value = value;
				IsUndefined = false;
			}
		}
		private TValue value;

		/// <summary>
		/// Флаг неопределённости значения
		/// </summary>
		public bool IsUndefined { get; private set; } = true;
		/// <summary>
		/// Закрыть текущее значение
		/// </summary>
		public void Close()
		{
			IsUndefined = true;
		}

		public override void Offset(TKey key)
		{
			if (Track.TryGetValue(key, out var value))
			{
				Value = value;
			}
			else
			{
				IsUndefined = true;
			}
		}

		public override void Revert()
		{
			if (Track.TryGetLastValue(out var value))
			{
				Value = value;
			}
			else
			{
				IsUndefined = true;
			}
		}

		public override void Commit(Transaction<TKey> transaction)
		{
			if (IsUndefined)
			{
				Track.Close(transaction);
			}
			else
			{
				Track.SetValue(Value, transaction); 
			}
		}
	}
}
