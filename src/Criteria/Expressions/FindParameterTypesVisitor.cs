using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Criteria.Expressions
{
	public class FindParameterTypesVisitor : ExpressionVisitor
	{
		private readonly IDictionary<Type, int> _parameterTypes;

		public FindParameterTypesVisitor()
		{
			_parameterTypes = new Dictionary<Type, int>();
		}

		public IDictionary<Type,int> FindParameterTypes(Expression expression)
		{
			_parameterTypes.Clear();

			Visit(expression);

			return _parameterTypes;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (_parameterTypes.ContainsKey(node.Type))
			{
				_parameterTypes[node.Type] += 1;
			}
			else
			{
				_parameterTypes.Add(node.Type, 1);
			}

			return base.VisitParameter(node);
		}
	}
}
