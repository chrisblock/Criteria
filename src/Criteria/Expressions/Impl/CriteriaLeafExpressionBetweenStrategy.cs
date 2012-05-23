using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Criteria.Json;
using Criteria.Registries;

using Newtonsoft.Json.Linq;

namespace Criteria.Expressions.Impl
{
	public class CriteriaLeafExpressionBetweenStrategy : ICriteriaLeafExpressionStrategy
	{
		private readonly ICriteriaTypeRegistry _typeRegistry;

		public CriteriaLeafExpressionBetweenStrategy(ICriteriaTypeRegistry typeRegistry)
		{
			_typeRegistry = typeRegistry;
		}

		public Expression GetExpression(ICriteriaLeaf leaf)
		{
			Expression result;

			var type = _typeRegistry.Lookup(leaf);

			var itemType = type.PropertyType;

			var accessorExpression = type.AccessorExpression;

			var valueCollection = leaf.Value as IEnumerable;

			try
			{
				var enumerator = valueCollection.GetEnumerator();
				enumerator.MoveNext();

				var firstItem = enumerator.Current;

				if ((firstItem.GetType().IsAssignableFrom(itemType) == false) && (itemType.IsInstanceOfType(firstItem) == false))
				{
					firstItem = GetType().GetMethod("GetTypedEnumerable").MakeGenericMethod(new[] { itemType }).Invoke(this, new[] { leaf.Value, 0 });
				}

				enumerator.MoveNext();
				var secondItem = enumerator.Current;

				if ((secondItem.GetType().IsAssignableFrom(itemType) == false) && (itemType.IsInstanceOfType(secondItem) == false))
				{
					secondItem = GetType().GetMethod("GetTypedEnumerable").MakeGenericMethod(new[] { itemType }).Invoke(this, new[] { leaf.Value, 1 });
				}

				var greaterThanLowerBound = Expression.GreaterThanOrEqual(accessorExpression, Expression.Constant(firstItem, itemType));

				var lessThanUpperBound = Expression.LessThanOrEqual(accessorExpression, Expression.Constant(secondItem, itemType));

				result = Expression.AndAlso(greaterThanLowerBound, lessThanUpperBound);
			}
			catch
			{
				throw new ArgumentException("Cannot use operator 'Between' on non-IEnumerable value.");
			}

			return result;
		}

		public T GetTypedEnumerable<T>(IEnumerable<JToken> value, int index)
		{
			try
			{
				var items = value.Select(x => x.Value<T>());
				return items.ElementAt(index);
			}
			catch (Exception)
			{
				throw new ArgumentException(String.Format("Could not convert {0} into type {1}", value, typeof(IEnumerable<T>).Name));
			}
		}
	}
}
