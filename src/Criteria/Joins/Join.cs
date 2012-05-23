using System.Linq.Expressions;

using Criteria.Expressions;
using Criteria.Registries;

namespace Criteria.Joins
{
	public class Join
	{
		private readonly IQueryableProvider _queryableProvider;
		private readonly ExpressionBuilder _expressionBuilder;
		private readonly IJoinPathRegistry _joinPathRegistry;

		private Join(IQueryableProvider queryableProvider, ExpressionBuilder expressionBuilder, IJoinPathRegistry joinPathRegistry)
		{
			_queryableProvider = queryableProvider;
			_expressionBuilder = expressionBuilder;
			_joinPathRegistry = joinPathRegistry;
		}

		public static Join Using(JoinConfiguration joinConfiguration)
		{
			return new Join(joinConfiguration.QueryableProvider, joinConfiguration.ExpressionBuilder, joinConfiguration.JoinPathRegistry);
		}

		public JoinPart StartWith<T>()
		{
			var type = typeof (T);

			var joinContext = new JoinContext
			{
				JoinPathRegistry = _joinPathRegistry,
				QueryProvider = _queryableProvider,
				JoinExpression = Expression.Constant(_queryableProvider.GetQueryableFor<T>()),
				LastJoinResultItemType = type
			};

			joinContext.GenericQueryableArguments.Add(type);

			return new JoinPart(joinContext, _expressionBuilder);
		}
	}
}
