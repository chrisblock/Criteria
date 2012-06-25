using System.Collections.Generic;

namespace Criteria.Registries
{
	public class JoinPathConfigurator<TOuter>
	{
		private readonly ICollection<JoinPath> _joinPaths;
		private readonly bool _isOneToMany;

		public JoinPathConfigurator(ICollection<JoinPath> joinPaths, bool isOneToMany = false)
		{
			_joinPaths = joinPaths;
			_isOneToMany = isOneToMany;
		}

		public JoinPathToConfigurator<TOuter, TInner> To<TInner>()
		{
			return new JoinPathToConfigurator<TOuter, TInner>(_joinPaths, _isOneToMany);
		}
	}
}
