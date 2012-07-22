using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Criteria.Expressions.Impl;
using Criteria.Json;
using Criteria.Registries;

namespace Criteria.Expressions
{
	public class ExpressionBuilder
	{
		private readonly ICriteriaLeafExpressionStrategy _leafExpressionStrategy;

		public ExpressionBuilder(ICriteriaTypeRegistry typeRegistry)
		{
			_leafExpressionStrategy = new CriteriaLeafExpressionStrategy(typeRegistry);
		}

		public Expression Build(JsonCriteriaNode jsonCriteriaNode)
		{
			var result = (jsonCriteriaNode.Operator.IsCompositeOperator())
							? BuildCompositeExpression(jsonCriteriaNode)
							: BuildLeafExpression(jsonCriteriaNode);

			return result;
		}

		private Expression BuildCompositeExpression(ICompositeCriteria criteria)
		{
			Expression body;

			if(criteria.Operator == Operator.And)
			{
				body = BuildBinaryExpressionTree(ExpressionType.AndAlso, criteria.Operands);
			}
			else if (criteria.Operator == Operator.Or)
			{
				body = BuildBinaryExpressionTree(ExpressionType.OrElse, criteria.Operands);
			}
			else
			{
				throw new ArgumentException(String.Format("Operator {0} is not a supported composite criteria operator type.", criteria.Operator));
			}

			return body;
		}

		private Expression BuildBinaryExpressionTree(ExpressionType type, IEnumerable<JsonCriteriaNode> operands)
		{
			if((operands == null) || (operands.Any() == false))
			{
				throw new ArgumentException(String.Format("Cannot apply operator {0} to no operands.", type));
			}

			var left = Build(operands.First());
			var right = operands.Skip(1).ToList();

			var result = (right.Any())
				? Expression.MakeBinary(type, left, BuildBinaryExpressionTree(type, right))
				: left;

			return result;
		}

		private Expression BuildLeafExpression(ICriteriaLeaf leaf)
		{
			var result = _leafExpressionStrategy.GetExpression(leaf);

			return result;
		}
	}
}
