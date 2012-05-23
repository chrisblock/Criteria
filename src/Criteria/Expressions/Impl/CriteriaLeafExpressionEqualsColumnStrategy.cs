using System;
using System.Linq.Expressions;

using Criteria.Json;
using Criteria.Registries;

namespace Criteria.Expressions.Impl
{
	public class CriteriaLeafExpressionEqualsColumnStrategy : ICriteriaLeafExpressionStrategy
	{
		private readonly ICriteriaTypeRegistry _typeRegistry;

		public CriteriaLeafExpressionEqualsColumnStrategy(ICriteriaTypeRegistry typeRegistry)
		{
			_typeRegistry = typeRegistry;
		}

		public Expression GetExpression(ICriteriaLeaf leaf)
		{
			var type = _typeRegistry.Lookup(leaf);

			var compareType = _typeRegistry.Lookup(String.Format("{0}", leaf.Value));

			var accessorExpression = type.AccessorExpression;
			var outputExpression = compareType.AccessorExpression;

			return Expression.Equal(accessorExpression, outputExpression);
		}
	}
}
