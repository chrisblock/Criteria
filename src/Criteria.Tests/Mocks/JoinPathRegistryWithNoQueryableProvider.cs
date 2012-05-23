using Criteria.Registries.Impl;
using Criteria.Tests.LinqToObjectsModel;

namespace Criteria.Tests.Mocks
{
	public class JoinPathRegistryWithNoQueryableProvider : BaseJoinPathRegistry
	{
		public JoinPathRegistryWithNoQueryableProvider()
		{
			RegisterOneTimeJoinFor<LinqToObjectsOne>(join => join.To<LinqToObjectsTwo>().On(p => p.LinqToObjectsOneId, c => c.LinqToObjectsOneParentId));
			RegisterOneTimeJoinFor<LinqToObjectsOne>(join => join.To<LinqToObjectsThree>().On(p => p.LinqToObjectsOneId, c => c.LinqToObjectsOneParentId));
		}
	}
}
