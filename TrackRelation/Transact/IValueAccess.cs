using System;
using System.Collections.Generic;
using System.Text;

namespace Track.Relation.Transact
{
	public interface IValueAccess<T>
	{
		T GetValue();
		void SetValue(T value);
	}
}
