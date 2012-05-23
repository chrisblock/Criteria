using System.Collections.Generic;

namespace Criteria.Registries
{
	public class JoinPathRegistryConfigurator<TOuter>
	{
		private readonly ICollection<JoinPath> _joinPaths;
		private readonly bool _isRepeatableJoin;

		public JoinPathRegistryConfigurator(ICollection<JoinPath> joinPaths) : this(joinPaths, false)
		{
		}

		public JoinPathRegistryConfigurator(ICollection<JoinPath> joinPaths, bool isRepeatableJoin)
		{
			_joinPaths = joinPaths;
			_isRepeatableJoin = isRepeatableJoin;
		}

		public JoinPathRegistryToConfigurator<TOuter, TInner> To<TInner>()
		{
			return new JoinPathRegistryToConfigurator<TOuter,TInner>(_joinPaths, _isRepeatableJoin);
		}

		public IEnumerable<JoinPath> GetJoinPaths()
		{
			return _joinPaths;
		}
	}
}
