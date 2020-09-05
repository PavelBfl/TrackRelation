using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation
{
	/// <summary>
	/// Поставщик ключей фиксации
	/// </summary>
	/// <typeparam name="T">Тип ключа фиксации</typeparam>
	public interface ICommitKeyProvider<T>
	{
		/// <summary>
		/// Создать новый ключ фиксации, каждый следующий ключ создаваемый ключ больше преведущего
		/// </summary>
		/// <returns>Ключ фиксации</returns>
		T CreateKey();
	}
}
