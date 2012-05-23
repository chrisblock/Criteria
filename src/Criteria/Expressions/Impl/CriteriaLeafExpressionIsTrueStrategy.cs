using System;
using System.Linq.Expressions;

using Criteria.Json;
using Criteria.Registries;

namespace Criteria.Expressions.Impl
{
	public class CriteriaLeafExpressionIsTrueStrategy : ICriteriaLeafExpressionStrategy
	{
		private readonly ICriteriaTypeRegistry _typeRegistry;

		public CriteriaLeafExpressionIsTrueStrategy(ICriteriaTypeRegistry typeRegistry)
		{
			_typeRegistry = typeRegistry;
		}

		public Expression GetExpression(ICriteriaLeaf leaf)
		{
			var type = _typeRegistry.Lookup(leaf);

			var itemType = type.PropertyType;

			if((itemType != typeof(bool)) && (itemType != typeof(bool?)))
			{
				throw new ArgumentException(String.Format("Cannot use the IsTrue operator with type '{0}'.", itemType));
			}

			return type.AccessorExpression;
		}
	}
}
