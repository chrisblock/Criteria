using System;
using System.Linq.Expressions;

using Criteria.Json;
using Criteria.Registries;

namespace Criteria.Expressions.Impl
{
	public class CriteriaLeafExpressionIsSpecifiedStrategy : ICriteriaLeafExpressionStrategy
	{
		private readonly ICriteriaTypeRegistry _typeRegistry;

		public CriteriaLeafExpressionIsSpecifiedStrategy(ICriteriaTypeRegistry typeRegistry)
		{
			_typeRegistry = typeRegistry;
		}

		public Expression GetExpression(ICriteriaLeaf leaf)
		{
			Expression result;

			var type = _typeRegistry.Lookup(leaf);

			var accessorExpression = type.AccessorExpression;

			var doesNotEqualNullExpression = Expression.NotEqual(accessorExpression, Expression.Constant(null));

			if (typeof(string).IsAssignableFrom(type.PropertyType))
			{
				var doesNotEqualEmptyStringExpression = Expression.NotEqual(accessorExpression, Expression.Constant(String.Empty, type.PropertyType));

				result = Expression.AndAlso(doesNotEqualNullExpression, doesNotEqualEmptyStringExpression);
			}
			else
			{
				result = doesNotEqualNullExpression;
			}

			return result;
		}
	}
}
