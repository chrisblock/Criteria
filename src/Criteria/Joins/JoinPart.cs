using System;
using System.Linq.Expressions;

using Criteria.Expressions;
using Criteria.Json;

namespace Criteria.Joins
{
	public class JoinPart : AbstractJoinPart
	{
		internal JoinPart(JoinContext joinContext, ExpressionBuilder expressionBuilder) : base(joinContext, expressionBuilder)
		{
		}

		public JoinToPart<T> Join<T>()
		{
			if(JoinContext.GenericQueryableArguments.Contains(typeof(T)) == false)
			{
				throw new ArgumentException(String.Format("Cannot join from type \"{0}\". That type is not part of the current result set.", typeof(T).Name));
			}

			return new JoinToPart<T>(JoinContext, ExpressionBuilder);
		}

		public ConstrainedJoinPart Where(JsonCriteriaNode criteria)
		{
			return new ConstrainedJoinPart(JoinContext, criteria, ExpressionBuilder);
		}

		public ConstrainedJoinPart Where<T>(Expression<Func<T, bool>> criteria)
		{
			if(JoinContext.GenericQueryableArguments.Contains(typeof(T)) == false)
			{
				throw new ArgumentException(String.Format("Cannot constrain on type \"{0}\". That type is not part of the current result set.", typeof(T).Name));
			}

			return new ConstrainedJoinPart(JoinContext, criteria);
		}

		protected override Expression WrapInConstraints(Type resultType, Expression joinExpression)
		{
			return joinExpression;
		}
	}
}
