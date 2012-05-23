﻿using System.Linq.Expressions;

using Criteria.Json;
using Criteria.Registries;

namespace Criteria.Expressions.Impl
{
	public class CriteriaLeafExpressionNotEqualsStrategy : ICriteriaLeafExpressionStrategy
	{
		private readonly ICriteriaTypeRegistry _typeRegistry;

		public CriteriaLeafExpressionNotEqualsStrategy(ICriteriaTypeRegistry typeRegistry)
		{
			_typeRegistry = typeRegistry;
		}

		public Expression GetExpression(ICriteriaLeaf leaf)
		{
			var type = _typeRegistry.Lookup(leaf);

			var typedValue = Converter.NullableSafeChangeType(leaf.Value, type.PropertyType);

			var accessorExpression = type.AccessorExpression;

			return Expression.NotEqual(accessorExpression, Expression.Constant(typedValue, type.PropertyType));
		}
	}
}
