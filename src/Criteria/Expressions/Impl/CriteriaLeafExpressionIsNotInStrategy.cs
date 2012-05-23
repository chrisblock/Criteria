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
	public class CriteriaLeafExpressionIsNotInStrategy : ICriteriaLeafExpressionStrategy
	{
		private readonly ICriteriaTypeRegistry _typeRegistry;

		public CriteriaLeafExpressionIsNotInStrategy(ICriteriaTypeRegistry typeRegistry)
		{
			_typeRegistry = typeRegistry;
		}

		public Expression GetExpression(ICriteriaLeaf leaf)
		{
			Expression result;

			var type = _typeRegistry.Lookup(leaf);

			var itemType = type.PropertyType;

			var accessorExpression = type.AccessorExpression;

			var itemCollection = leaf.Value as IEnumerable;

			if(itemCollection != null)
			{
				Expression arrayExpression;

				// TODO: try to figure out if the IEnumerable.Cast<> method will handle this case also
				if(itemCollection.GetType().IsAssignableFrom(typeof(JArray)) || (itemCollection is JArray))
				{
					arrayExpression = (Expression)GetType().GetMethod("BuildArrayInitializationExpression").MakeGenericMethod(new[] { itemType }).Invoke(this, new[] { leaf.Value });
				}
				else
				{
					arrayExpression = (Expression)GetType().GetMethod("BuildConstantArrayExpression").MakeGenericMethod(new[] { itemType }).Invoke(this, new object[] { itemCollection });
				}

				result = Expression.Not(Expression.Call(typeof(Enumerable), "Contains", new[] { itemType }, new[] { arrayExpression, accessorExpression }));
			}
			else
			{
				throw new ArgumentException("Cannot use operator 'IsNotIn' on non-IEnumerable value.");
			}

			return result;
		}

		public Expression BuildArrayInitializationExpression<T>(JArray value)
		{
			var items = value.AsEnumerable().Select(x => x.Value<T>());

			if(items == null)
			{
				throw new ArgumentException(String.Format("Could not convert {0} into type {1}", value, typeof (IEnumerable<T>).Name));
			}

			var newArrayExpression = Expression.NewArrayInit(typeof (T), items.Select(x => (Expression)Expression.Constant(x, typeof (T))));

			return newArrayExpression;
		}

		public Expression BuildConstantArrayExpression<T>(IEnumerable itemCollection)
		{
			return Expression.Constant(itemCollection.Cast<T>().ToArray());
		}
	}
}
