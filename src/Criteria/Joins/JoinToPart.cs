using System;

using Criteria.Expressions;

namespace Criteria.Joins
{
	public class JoinToPart<T>
	{
		private readonly JoinContext _joinContext;
		private readonly ExpressionBuilder _expressionBuilder;

		internal JoinToPart(JoinContext joinContext, ExpressionBuilder expressionBuilder)
		{
			_joinContext = joinContext;
			_expressionBuilder = expressionBuilder;
		}

		public JoinOnPart<T, TInner> To<TInner>()
		{
			if(typeof(T) == typeof(TInner))
			{
				throw new ArgumentException(String.Format("Cannot join from type \"{0}\" to type \"{1}\". That's stupid.", typeof(T), typeof(TInner)));
			}

			return new JoinOnPart<T, TInner>(_joinContext, _expressionBuilder);
		}
	}
}
