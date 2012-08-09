using System.Linq;

namespace Criteria
{
	public interface IQueryableProvider
	{
		IQueryable<T> GetQueryableFor<T>();
	}
}
