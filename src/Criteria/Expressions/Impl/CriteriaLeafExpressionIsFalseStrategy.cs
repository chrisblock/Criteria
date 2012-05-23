using System;
using System.Linq.Expressions;

using Criteria.Json;
using Criteria.Registries;

namespace Criteria.Expressions.Impl
{
	public class CriteriaLeafExpressionIsFalseStrategy : ICriteriaLeafExpressionStrategy
	{
		private readonly ICriteriaTypeRegistry _typeRegistry;

		public CriteriaLeafExpressionIsFalseStrategy(ICriteriaTypeRegistry typeRegistry)
		{
			_typeRegistry = typeRegistry;
		}

		public Expression GetExpression(ICriteriaLeaf leaf)
		{
			var type = _typeRegistry.Lookup(leaf);

			var itemType = type.PropertyType;

			if((itemType != typeof(bool)) && (itemType != typeof(bool?)))
			{
				throw new ArgumentException(String.Format("Cannot use the IsFalse operator with type '{0}'.", itemType));
			}

			return Expression.Not(type.AccessorExpression);
		}
	}
}
