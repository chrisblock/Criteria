using System;
using System.Linq.Expressions;

using Criteria.Json;
using Criteria.Registries;

namespace Criteria.Expressions.Impl
{
	public class CriteriaLeafExpressionDoesNotEndWithStrategy : ICriteriaLeafExpressionStrategy
	{
		private readonly ICriteriaTypeRegistry _typeRegistry;

		public CriteriaLeafExpressionDoesNotEndWithStrategy(ICriteriaTypeRegistry typeRegistry)
		{
			_typeRegistry = typeRegistry;
		}

		public Expression GetExpression(ICriteriaLeaf leaf)
		{
			var type = _typeRegistry.Lookup(leaf);

			if(type.PropertyType != typeof(string))
			{
				throw new ArgumentException(String.Format("Operator {0} is not defined for non-string properties.", leaf.Operator));
			}

			var accessorExpression = type.AccessorExpression;

			var x = Convert.ChangeType(leaf.Value, type.PropertyType);

			return Expression.Not(Expression.Call(accessorExpression, "EndsWith", new Type[0], new Expression[] { Expression.Constant(x, type.PropertyType) }));
		}
	}
}
