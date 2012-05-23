﻿using System;
using System.Linq.Expressions;

using Criteria.Json;
using Criteria.Registries;

namespace Criteria.Expressions.Impl
{
	public class CriteriaLeafExpressionDoesNotStartWithStrategy : ICriteriaLeafExpressionStrategy
	{
		private readonly ICriteriaTypeRegistry _typeRegistry;

		public CriteriaLeafExpressionDoesNotStartWithStrategy(ICriteriaTypeRegistry typeRegistry)
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

			return Expression.Not(Expression.Call(accessorExpression, "StartsWith", new Type[0], new Expression[] { Expression.Constant(typedValue, type.PropertyType) }));
		}
	}
}
