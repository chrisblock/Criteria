using System.Linq.Expressions;

using Criteria.Json;

namespace Criteria.Expressions
{
	public interface ICriteriaLeafExpressionStrategy
	{
		Expression GetExpression(ICriteriaLeaf leaf);
	}
}
