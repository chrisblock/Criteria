using System;
using System.Linq;
using System.Linq.Expressions;

using Criteria.Expressions;
using Criteria.Json;

namespace Criteria.Joins
{
	public class ConstrainedJoinPart : AbstractJoinPart
	{
		private readonly ExpressionBuilder _expressionBuilder;

		private LambdaExpression _whereClause;

		internal ConstrainedJoinPart(JoinContext joinContext, JsonCriteriaNode criteria, ExpressionBuilder expressionBuilder) : base(joinContext, expressionBuilder)
		{
			_expressionBuilder = expressionBuilder;
			_whereClause = BuildWhereClause(criteria);
		}

		internal ConstrainedJoinPart(JoinContext joinContext, LambdaExpression criteria) : base(joinContext)
		{
			_whereClause = BuildWhereClause(criteria);
		}

		public ConstrainedJoinPart Where<T>(Expression<Func<T, bool>> additionalConstraint)
		{
			AddConstraint(additionalConstraint.Body);

			return this;
		}

		public ConstrainedJoinPart Where(JsonCriteriaNode additionalCriteria)
		{
			AddConstraint(_expressionBuilder.Build(additionalCriteria));

			return this;
		}

		private void AddConstraint(Expression expression)
		{
			var parameterExpression = _whereClause.Parameters.Single();

			var newWhereClause = InjectJoinedTypePropertyVisitor.CreateInjectorForParameter(parameterExpression, JoinContext.JoinPathRegistry).Inject(expression);

			var body = Expression.AndAlso(_whereClause.Body, newWhereClause);

			_whereClause = Expression.Lambda(body, false, _whereClause.Parameters);
		}

		private LambdaExpression BuildWhereClause(JsonCriteriaNode criteria)
		{
			return InjectParameterForWhereClause(_expressionBuilder.Build(criteria));
		}

		private LambdaExpression BuildWhereClause(LambdaExpression criteria)
		{
			return InjectParameterForWhereClause(criteria.Body);
		}

		private LambdaExpression InjectParameterForWhereClause(Expression x)
		{
			var parameterExpression = Expression.Parameter(JoinContext.LastJoinResultItemType, JoinContext.GetNextParameterName());

			var body = InjectJoinedTypePropertyVisitor.CreateInjectorForParameter(parameterExpression,JoinContext.JoinPathRegistry).Inject(x);

			var lambda = Expression.Lambda(body, false, new[] { parameterExpression });

			return lambda;
		}

		protected override Expression WrapInConstraints(Type resultType, Expression joinExpression)
		{
			var joinExpressionReturnItemType = JoinContext.LastJoinResultItemType;

			// TODO: apply Where clauses when they are input, like joins??
			//       are multiple where's better or worse performance than one hueg one?? does that even matter??
			Expression constrainedExpression = Expression.Call(typeof(Queryable), "Where", new[] { joinExpressionReturnItemType }, new[] { joinExpression, _whereClause });

			return constrainedExpression;
		}
	}
}
