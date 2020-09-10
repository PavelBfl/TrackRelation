using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Tracks
{
	/// <summary>
	/// Объект фиксации данных
	/// </summary>
	/// <typeparam name="TKey">Тип ключа фиксации</typeparam>
	/// <typeparam name="TValue">Тип значения фиксации</typeparam>
	public interface ICommit<TKey, TValue>
	{
		/// <summary>
		/// Ключ
		/// </summary>
		TKey Key { get; }
		/// <summary>
		/// Значение
		/// </summary>
		TValue Value { get; }
	}
}
