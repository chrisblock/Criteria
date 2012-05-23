using System;
using System.Linq.Expressions;

using Criteria.Json;
using Criteria.Registries;

namespace Criteria.Expressions.Impl
{
	public class CriteriaLeafExpressionIsNotSpecifiedStrategy : ICriteriaLeafExpressionStrategy
	{
		private readonly ICriteriaTypeRegistry _typeRegistry;

		public CriteriaLeafExpressionIsNotSpecifiedStrategy(ICriteriaTypeRegistry typeRegistry)
		{
			_typeRegistry = typeRegistry;
		}

		public Expression GetExpression(ICriteriaLeaf leaf)
		{
			Expression result;

			var type = _typeRegistry.Lookup(leaf);

			var accessorExpression = type.AccessorExpression;

			var equalsNullExpression = Expression.Equal(accessorExpression, Expression.Constant(null));

			if (typeof(string).IsAssignableFrom(type.PropertyType))
			{
				var equalsEmptyStringExpression = Expression.Equal(accessorExpression, Expression.Constant(String.Empty, type.PropertyType));

				result = Expression.OrElse(equalsNullExpression, equalsEmptyStringExpression);
			}
			else
			{
				result = equalsNullExpression;
			}

			return result;
		}
	}
}
