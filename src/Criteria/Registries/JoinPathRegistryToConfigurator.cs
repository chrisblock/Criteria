using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Criteria.Registries
{
	public class JoinPathRegistryToConfigurator<TOuter, TInner>
	{
		private readonly ICollection<JoinPath> _joinPaths;
		private readonly bool _isRepeatableJoin;

		public JoinPathRegistryToConfigurator(ICollection<JoinPath> joinPaths, bool isRepeatableJoin)
		{
			_joinPaths = joinPaths;
			_isRepeatableJoin = isRepeatableJoin;
		}

		public void On<TKey>(Expression<Func<TOuter, TKey>> outerKey, Expression<Func<TInner, TKey>> innerKey)
		{
			var dependency = JoinPath.Create(outerKey, innerKey, _isRepeatableJoin);
			var inverse = JoinPath.Create(innerKey, outerKey, _isRepeatableJoin);

			_joinPaths.Add(dependency);
			_joinPaths.Add(inverse);
		}
	}
}
