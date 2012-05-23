﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

using Criteria.Expressions;
using Criteria.Sql;

using StructureMap;

namespace Criteria.Joins
{
	public abstract class AbstractJoinPart
	{
		protected readonly JoinContext JoinContext;
		protected readonly ExpressionBuilder ExpressionBuilder;

		protected AbstractJoinPart(JoinContext joinContext, ExpressionBuilder expressionBuilder)
		{
			JoinContext = joinContext;
			ExpressionBuilder = expressionBuilder;
		}

		protected AbstractJoinPart(JoinContext joinContext) : this(joinContext, null)
		{
		}

		protected abstract Expression WrapInConstraints(Type resultType, Expression joinExpression);

		private Expression WrapInSingleTypeProjection(Type resultType, Expression joinExpression)
		{
			Expression result = joinExpression;

			if(JoinContext.GenericQueryableArguments.Count > 1)
			{
				var resultProjectionExpression = GetSingleTypeResultProjectionExpression(resultType);

				result = Expression.Call(typeof(Queryable), "Select", new[] { JoinContext.LastJoinResultItemType, resultType }, new[] { joinExpression, resultProjectionExpression });
			}
			else if(JoinContext.GenericQueryableArguments.Single() != resultType)
			{
				throw new ArgumentException(String.Format("Cannot select type \"{0}\". That type is not part of the current result set.", resultType.Name));
			}

			return result;
		}

		private Expression WrapInCompositeTypeProjection(Type resultType, Expression joinExpression)
		{
			Expression result = joinExpression;

			if (JoinContext.GenericQueryableArguments.Count > 1)
			{
				var resultProjectionExpression = GetCompositeTypeResultProjectionExpression(resultType);

				result = Expression.Call(typeof(Queryable), "Select", new[] { JoinContext.LastJoinResultItemType, resultType }, new[] { joinExpression, resultProjectionExpression });
			}
			else if (JoinContext.GenericQueryableArguments.Single() != resultType)
			{
				throw new ArgumentException(String.Format("Cannot select type \"{0}\". That type is not part of the current result set.", resultType.Name));
			}

			return result;
		}

		private LambdaExpression GetSingleTypeResultProjectionExpression(Type resultType)
		{
			var previousJoinType = JoinContext.LastJoinResultItemType;

			var resultProjectionParameter = Expression.Parameter(previousJoinType, JoinContext.GetNextParameterName());

			var resultProjectionField = previousJoinType
				.GetFields()
				.SingleOrDefault(x => x.FieldType == resultType);

			if(resultProjectionField == null)
			{
				throw new ArgumentException(String.Format("Cannot select type \"{0}\". That type is not part of the current result set.", resultType.Name));
			}

			var resultProjectionExpression = Expression.Lambda(Expression.Field(resultProjectionParameter, resultProjectionField), false, new[] { resultProjectionParameter });

			return resultProjectionExpression;
		}

		private LambdaExpression GetCompositeTypeResultProjectionExpression(Type resultType)
		{
			LambdaExpression result;

			var inputType = JoinContext.LastJoinResultItemType;
			var outputType = resultType;

			var knownTypes = inputType.GetFields().Select(x => x.FieldType);
			var knownTypeFieldAccessorExpressions = inputType.GetFields().ToDictionary(k => k.FieldType, v => v);

			var constructorsWithOnlyParametersOfKnownType = outputType
				.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
				.Where(x => x.GetParameters().All(p => knownTypes.Contains(p.ParameterType)))
				.OrderByDescending(x => x.GetParameters().Count());

			if (constructorsWithOnlyParametersOfKnownType.Any())
			{
				var constructorWithTheMostParameters = constructorsWithOnlyParametersOfKnownType.First();

				var inputExpression = Expression.Parameter(inputType, JoinContext.GetNextParameterName());

				var arguments = constructorWithTheMostParameters.GetParameters()
					.Select(x => Expression.Field(inputExpression, knownTypeFieldAccessorExpressions[x.ParameterType]));

				var body = Expression.New(constructorWithTheMostParameters, arguments);

				result = Expression.Lambda(body, false, new[] { inputExpression });
			}
			else
			{
				throw new ArgumentException(String.Format("Type {0} does not contain a constructor that accepts only types in the result set.", outputType));
			}

			return result;
		}

		private Expression BuildBodyExpression(Type resultType)
		{
			var expression = WrapInConstraints(resultType, JoinContext.JoinExpression);
			expression = WrapInSingleTypeProjection(resultType, expression);

			return expression;
		}

		public IQueryable<TResult> Query<TResult>()
		{
			var expression = BuildBodyExpression(typeof(TResult));

			var lambda = Expression.Lambda(expression, false);

			var func = lambda.Compile();

			var queryable = func.DynamicInvoke() as IQueryable;

			if(queryable == null)
			{
				throw new Exception(String.Format("Could not cast result of LambdaExpression:\n\n{0}\n\nas an IQueryable.", lambda));
			}

			return queryable
				.Cast<TResult>();
		}

		public IQueryable<TResult> Project<TResult>()
		{
			var expression = WrapInConstraints(typeof(TResult), JoinContext.JoinExpression);

			expression = WrapInCompositeTypeProjection(typeof (TResult), expression);

			var lambda = Expression.Lambda(expression, false);

			var func = lambda.Compile();

			var queryable = func.DynamicInvoke() as IQueryable;

			if (queryable == null)
			{
				throw new Exception(String.Format("Could not cast result of LambdaExpression:\n\n{0}\n\nas an IQueryable.", lambda));
			}

			return queryable
				.Cast<TResult>();
		}

		public SqlGeneratorResult Sql<TResult>()
		{
			return Sql<TResult>(new SqlGeneratorConfiguration());
		}

		public SqlGeneratorResult Sql<TResult>(Action<SqlGeneratorConfigurator> configure)
		{
			var configuration = new SqlGeneratorConfiguration();

			var configurator = new SqlGeneratorConfigurator(configuration);

			configure(configurator);

			return Sql<TResult>(configuration);
		}

		private SqlGeneratorResult Sql<TResult>(SqlGeneratorConfiguration configuration)
		{
			var resultType = typeof(TResult);

			var body = BuildBodyExpression(resultType);

			var sqlGenerator = ObjectFactory.GetInstance<ISqlGenerator>();

			if (configuration.DistinctCountPropertyExpression == null)
			{
				if (configuration.Distinct)
				{
					body = Expression.Call(typeof (Queryable), "Distinct", new[] {resultType}, body);
				}
				else if (configuration.Count)
				{
					body = Expression.Call(typeof (Queryable), "Count", new[] {resultType}, body);
				}
			}
			else
			{
				if (configuration.DistinctCountPropertyExpression.Parameters.Single().Type != resultType)
				{
					throw new ArgumentException(String.Format("Cannot count distinct for type '{0}' with projection '{1}'", resultType, configuration.DistinctCountPropertyExpression));
				}

				var distinctProjectionExpressionResultType = configuration.DistinctCountPropertyExpression.ReturnType;

				body = Expression.Call(typeof(Queryable), "Select", new[] { resultType, distinctProjectionExpressionResultType }, body, configuration.DistinctCountPropertyExpression);

				body = Expression.Call(typeof(Queryable), "Distinct", new[] { distinctProjectionExpressionResultType }, body);

				// TODO: fix this so that it works and so that matt wont have to burn in hell for his transgression below
				//body = Expression.Call(typeof(Queryable), "Count", new[] { distinctProjectionExpressionResultType }, body);
			}

			SqlGeneratorResult result;

			if (configuration.Unalias)
			{
				result = sqlGenerator.GenerateUnaliased(body);
			}
			else if(configuration.StarSelect)
			{
				result = sqlGenerator.GenerateStarSelect(body);
			}
			else
			{
				result = sqlGenerator.Generate(body);
			}

			// TODO: matt will burn in hell for this
			if(configuration.DistinctCountPropertyExpression != null)
			{
				result.SqlString = Regex.Replace(result.SqlString, @"(distinct\s+\S+)", "cast(count($1) as INT)", RegexOptions.IgnoreCase);
			}

			return result;
		}
	}
}
