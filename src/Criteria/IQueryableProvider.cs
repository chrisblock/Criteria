using System;
using System.Linq;

namespace Criteria
{
	public interface IQueryableProvider
	{
		IQueryable<T> GetQueryableFor<T>();
		IQueryable GetQueryable(Type type);
	}
}
