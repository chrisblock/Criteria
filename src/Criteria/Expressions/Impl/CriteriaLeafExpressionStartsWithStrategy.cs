using System;
using System.Linq.Expressions;

using Criteria.Json;
using Criteria.Registries;

namespace Criteria.Expressions.Impl
{
	public class CriteriaLeafExpressionStartsWithStrategy : ICriteriaLeafExpressionStrategy
	{
		private readonly ICriteriaTypeRegistry _typeRegistry;

		public CriteriaLeafExpressionStartsWithStrategy(ICriteriaTypeRegistry typeRegistry)
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

			var typedValue = Convert.ChangeType(leaf.Value, type.PropertyType);

			var accessorExpression = type.AccessorExpression;

			return Expression.Call(accessorExpression, "StartsWith", Type.EmptyTypes, new Expression[] { Expression.Constant(typedValue, type.PropertyType) });
		}
	}
}
