using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Criteria.Json;
using Criteria.Registries;

namespace Criteria.Expressions.Impl
{
	public class CriteriaLeafExpressionStrategy : ICriteriaLeafExpressionStrategy
	{
		private readonly IDictionary<Operator, ICriteriaLeafExpressionStrategy> _criteriaLeafExpressionStrategies;

		private readonly ICriteriaTypeRegistry _typeRegistry;

		public CriteriaLeafExpressionStrategy(ICriteriaTypeRegistry typeRegistry)
		{
			_typeRegistry = typeRegistry;

			_criteriaLeafExpressionStrategies = new Dictionary<Operator, ICriteriaLeafExpressionStrategy>
			{
				{Operator.Equal, new CriteriaLeafExpressionEqualsStrategy(_typeRegistry)},
				{Operator.NotEqual, new CriteriaLeafExpressionNotEqualsStrategy(_typeRegistry)},
				{Operator.LessThan, new CriteriaLeafExpressionLessThanStrategy(_typeRegistry)},
				{Operator.LessThanOrEqual, new CriteriaLeafExpressionLessThanOrEqualToStrategy(_typeRegistry)},
				{Operator.GreaterThan, new CriteriaLeafExpressionGreaterThanStrategy(_typeRegistry)},
				{Operator.GreaterThanOrEqual, new CriteriaLeafExpressionGreaterThanOrEqualToStrategy(_typeRegistry)},
				{Operator.IsIn, new CriteriaLeafExpressionIsInStrategy(_typeRegistry)},
				{Operator.IsNotIn, new CriteriaLeafExpressionIsNotInStrategy(_typeRegistry)},
				{Operator.Between, new CriteriaLeafExpressionBetweenStrategy(_typeRegistry)},
				{Operator.Contains, new CriteriaLeafExpressionContainsStrategy(_typeRegistry)},
				{Operator.DoesNotContain, new CriteriaLeafExpressionDoesNotContainStrategy(_typeRegistry)},
				{Operator.StartsWith, new CriteriaLeafExpressionStartsWithStrategy(_typeRegistry)},
				{Operator.DoesNotStartWith, new CriteriaLeafExpressionDoesNotStartWithStrategy(_typeRegistry)},
				{Operator.EndsWith, new CriteriaLeafExpressionEndsWithStrategy(_typeRegistry)},
				{Operator.DoesNotEndWith, new CriteriaLeafExpressionDoesNotEndWithStrategy(_typeRegistry)},
				{Operator.IsTrue, new CriteriaLeafExpressionIsTrueStrategy(_typeRegistry)},
				{Operator.IsFalse, new CriteriaLeafExpressionIsFalseStrategy(_typeRegistry)},
				{Operator.IsNotSpecified, new CriteriaLeafExpressionIsNotSpecifiedStrategy(_typeRegistry)},
				{Operator.EqualsColumn, new CriteriaLeafExpressionEqualsColumnStrategy(_typeRegistry)},
				{Operator.IsSpecified, new CriteriaLeafExpressionIsSpecifiedStrategy(_typeRegistry)}
			};
		}

		public Expression GetExpression(ICriteriaLeaf leaf)
		{
			ICriteriaLeafExpressionStrategy strategy;

			if(_criteriaLeafExpressionStrategies.TryGetValue(leaf.Operator, out strategy) == false)
			{
				throw new ArgumentException(String.Format("No CriteriaLeafExpressionStrategy for operator type {0}", leaf.Operator));
			}

			var result = strategy.GetExpression(leaf);

			return result;
		}
	}
}
