using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Criteria.Registries
{
	public abstract class JoinPathToConfigurator
	{
		public abstract Type InnerType { get; }
		public abstract Type OuterType { get; }
	}

	public class JoinPathToConfigurator<TOuter, TInner> : JoinPathToConfigurator
	{
		private readonly ICollection<JoinPath> _joinPaths;
		private readonly bool _isOneToMany;

		public override Type InnerType { get { return typeof (TOuter); } }
		public override Type OuterType { get { return typeof (TInner); } }

		public JoinPathToConfigurator(ICollection<JoinPath> joinPaths, bool isOneToMany)
		{
			_joinPaths = joinPaths;
			_isOneToMany = isOneToMany;
		}

		public JoinPathToConfigurator<TOuter, TInner> On<TKey>(Expression<Func<TOuter, TKey>> outerKey, Expression<Func<TInner, TKey>> innerKey)
		{
			var dependency = JoinPath.Create(outerKey, innerKey, _isOneToMany);
			var inverse = JoinPath.Create(innerKey, outerKey, _isOneToMany);

			_joinPaths.Add(dependency);
			_joinPaths.Add(inverse);

			return this;
		}
	}
}
