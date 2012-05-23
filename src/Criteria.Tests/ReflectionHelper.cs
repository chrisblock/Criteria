﻿using System;
using System.Linq.Expressions;

namespace Criteria.Tests
{
	public static class ReflectionHelper
	{
		public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> accessorLambda)
		{
			var lambda = accessorLambda as LambdaExpression;

			if (lambda == null)
			{
				throw new ArgumentException(String.Format("Cannot convert '{0}' to a LambdaExpression.", accessorLambda));
			}

			var accessorExpression = lambda.Body as MemberExpression;

			if(accessorExpression == null)
			{
				throw new ArgumentException(String.Format("Cannot convert '{0}' to a MemberExpression.", lambda.Body));
			}

			return accessorExpression.Member.Name;
		}
	}
}