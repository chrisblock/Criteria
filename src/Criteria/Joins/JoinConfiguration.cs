using Criteria.Expressions;
using Criteria.Registries;

namespace Criteria.Joins
{
	public class JoinConfiguration
	{
		public IQueryableProvider QueryableProvider { get; set; }
		public ExpressionBuilder ExpressionBuilder { get; set; }
		public IJoinPathRegistry JoinPathRegistry { get; set; }
	}
}
