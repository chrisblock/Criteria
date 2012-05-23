using System.Collections.Generic;

namespace Criteria.Registries
{
	public class JoinPathConfigurator<TOuter>
	{
		private readonly ICollection<JoinPath> _joinPaths;
		private readonly bool _createsOneToManyJoins;

		public JoinPathConfigurator(ICollection<JoinPath> joinPaths, bool createsOneToManyJoins = false)
		{
			_joinPaths = joinPaths;
			_createsOneToManyJoins = createsOneToManyJoins;
		}

		public JoinPathToConfigurator<TOuter, TInner> To<TInner>()
		{
			return new JoinPathToConfigurator<TOuter, TInner>(_joinPaths, _createsOneToManyJoins);
		}
	}
}
