using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Criteria.Registries;

namespace Criteria.Joins
{
	/// <summary>
	/// This visitor assumes that the initial expression is a lambda expression where its body is only a member access expression.
	/// This visitor also assumes that the fieldAccessExpression is also a member access expression
	/// This visitor also assumes the existance of a Join Path Registry
	/// </summary>
	public class InjectJoinedTypePropertyVisitor : ExpressionVisitor
	{
		private readonly ParameterExpression _parameterToInject;
		private readonly List<FieldInfo> _parameterFieldTypes;
		private readonly Dictionary<Type, bool> _multipleJoinLookup;

		public static InjectJoinedTypePropertyVisitor CreateInjectorForParameter(ParameterExpression parameterToInject, IJoinPathRegistry joinPathRegistry)
		{
			return new InjectJoinedTypePropertyVisitor(parameterToInject, joinPathRegistry);
		}

		private InjectJoinedTypePropertyVisitor(ParameterExpression parameterToInject, IJoinPathRegistry joinPathRegistry)
		{
			_parameterToInject = parameterToInject;
			_parameterFieldTypes = parameterToInject.Type.GetFields().ToList();
			_multipleJoinLookup = joinPathRegistry.MultipleJoinLookup;
		}

		public Expression Inject(Expression initial)
		{
			return Visit(initial);
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			Expression result;

			if (_parameterToInject.Type == node.Type)
			{
				result = _parameterToInject;
			}
			else
			{
				var parameterField = _parameterFieldTypes.FirstOrDefault(x => x.FieldType == node.Type);
				
				if (parameterField == null)
				{
					throw new ArgumentException(String.Format("Could not find field of type {0} as part of type {1}", node.Type, _parameterToInject.Type));
				}

				result = Expression.Field(_parameterToInject, parameterField);
				if (_multipleJoinLookup.ContainsKey(node.Type))
				{
					_parameterFieldTypes.Remove(parameterField);
				}
			}

			return result;
		}
	}
}
