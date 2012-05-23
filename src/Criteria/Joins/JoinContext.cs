using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Criteria.Registries;

namespace Criteria.Joins
{
	public class JoinContext
	{
		public IQueryableProvider QueryProvider { get; set; }
		public Expression JoinExpression { get; set; }
		public IJoinPathRegistry JoinPathRegistry { get; set; }

		public Type LastJoinResultItemType { get; set; }

		public IList<Type> GenericQueryableArguments { get; private set; }

		public JoinContext()
		{
			GenericQueryableArguments = new List<Type>();
		}

		private int _parameterNumber;
		public string GetNextParameterName()
		{
			return String.Format("param_{0}", _parameterNumber++);
		}
	}
}
