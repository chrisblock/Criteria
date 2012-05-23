using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Criteria.Expressions;

namespace Criteria.Joins
{
	public class JoinOnPart<TOuter, TInner>
	{
		private readonly JoinContext _joinContext;
		private readonly ExpressionBuilder _expressionBuilder;

		internal JoinOnPart(JoinContext joinContext, ExpressionBuilder expressionBuilder)
		{
			_joinContext = joinContext;
			_expressionBuilder = expressionBuilder;

			_joinContext.GenericQueryableArguments.Add(typeof(TInner));
		}

		public JoinPart On<TKey>(Expression<Func<TOuter, TKey>> outerKey, Expression<Func<TInner, TKey>> innerKey)
		{
			_joinContext.JoinExpression = BuildJoinExpression(outerKey, innerKey);

			var result = new JoinPart(_joinContext, _expressionBuilder);

			return result;
		}

		private Expression BuildJoinExpression<TKey>(Expression<Func<TOuter, TKey>> outerKey, Expression<Func<TInner, TKey>> innerKey)
		{
			var resultSelector = BuildResultSelectorExpression();

			var fixedOuterKey = GetOuterKeyExpression(outerKey);

			var previousJoinReturnType = _joinContext.LastJoinResultItemType;

			var outerQueryableExpression = _joinContext.JoinExpression;

			var joinTypeParameters = new[] { previousJoinReturnType, typeof(TInner), typeof(TKey), resultSelector.ReturnType };

			var innerQueryableExpression = Expression.Constant(_joinContext.QueryProvider.GetQueryableFor<TInner>());

			var joinParameters = new[] {outerQueryableExpression, innerQueryableExpression, fixedOuterKey, innerKey, resultSelector};

			_joinContext.LastJoinResultItemType = resultSelector.ReturnType;

			var joinExpression = Expression.Call(typeof(Queryable), "Join", joinTypeParameters, joinParameters);

			return joinExpression;
		}

		private LambdaExpression GetOuterKeyExpression(LambdaExpression currentOuterKey)
		{
			var returnType = _joinContext.LastJoinResultItemType;

			var outerKeyParameter = Expression.Parameter(returnType, _joinContext.GetNextParameterName());

			var injector = InjectJoinedTypePropertyVisitor.CreateInjectorForParameter(outerKeyParameter, _joinContext.JoinPathRegistry);

			var fixedOuterKeyBody = injector.Inject(currentOuterKey.Body);

			var fixedOuterKey = Expression.Lambda(fixedOuterKeyBody, false, new[] { outerKeyParameter });

			return fixedOuterKey;
		}

		private LambdaExpression BuildResultSelectorExpression()
		{
			var firstParameterType = _joinContext.LastJoinResultItemType;
			var firstParameter = GenerateParameterFromType(firstParameterType);

			var secondParameter = GenerateParameterFromType(typeof(TInner));

			var selectParameterExpressions = new[] { firstParameter, secondParameter };

			var resultType = GenerateAnonymousType(firstParameter.Type, secondParameter.Type);

			var newAnonymousTypeFieldAssignmentExpressions = GenerateAnonymousTypeFieldAssignmentExpressions(resultType.GetFields(), firstParameter, secondParameter);

			var body = Expression.MemberInit(Expression.New(resultType), newAnonymousTypeFieldAssignmentExpressions);

			var resultSelector = Expression.Lambda(body, false, selectParameterExpressions);

			return resultSelector;
		}

		private ParameterExpression GenerateParameterFromType(Type parameterType)
		{
			var parameterName = _joinContext.GetNextParameterName();

			return Expression.Parameter(parameterType, parameterName);
		}

		private static Type GenerateAnonymousType(Type firstParameterType, Type secondParameterType)
		{
			IEnumerable<Type> fieldTypes;

			if (firstParameterType == typeof(TOuter))
			{
				fieldTypes = new[] {firstParameterType, secondParameterType};
			}
			else
			{
				fieldTypes = firstParameterType
								.GetFields()
								.Select(x => x.FieldType)
								.Concat(new[] { secondParameterType });
			}

			var resultType = AnonymousClassManager.BuildNewAnonymousTypeWithFields(fieldTypes);

			return resultType;
		}

		private IEnumerable<MemberAssignment> GenerateAnonymousTypeFieldAssignmentExpressions(IEnumerable<FieldInfo> fields, ParameterExpression firstParameter, ParameterExpression secondParameter)
		{
			var targetFields = fields.ToList();
			var results = new List<MemberAssignment>();

			if (firstParameter.Type == typeof(TOuter))
			{
				results.Add(Expression.Bind(targetFields.Single(x => x.FieldType == firstParameter.Type), firstParameter));
			}
			else
			{
				var availableFields = firstParameter.Type
					.GetFields()
					.ToList();

				foreach(var target in targetFields.ToList())
				{
					if(availableFields.Select(x => x.FieldType).Contains(target.FieldType))
					{
						var matchedField = availableFields.First(x => x.FieldType == target.FieldType);
						results.Add(Expression.Bind(target, Expression.Field(firstParameter, matchedField)));
						availableFields.Remove(matchedField);
						targetFields.Remove(target);
					}
				}
			}

			results.Add(Expression.Bind(targetFields.Single(x => x.FieldType == secondParameter.Type), secondParameter));

			return results;
		}
	}
}
