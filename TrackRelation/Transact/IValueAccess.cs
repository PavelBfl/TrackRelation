using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Transact
{
	/// <summary>
	/// Интерфейс доступа к отслеживаемым данным
	/// </summary>
	/// <typeparam name="T">Тип отслеживаемых данных</typeparam>
	public interface IValueAccess<T>
	{
		/// <summary>
		/// Получить значение данных
		/// </summary>
		/// <returns>Значение</returns>
		T GetValue();
		/// <summary>
		/// Назначить значение
		/// </summary>
		/// <param name="value">Новое значение</param>
		void SetValue(T value);
	}
}
