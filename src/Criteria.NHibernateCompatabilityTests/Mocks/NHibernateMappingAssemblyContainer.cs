using Criteria.Sql.Impl;

namespace Criteria.NHibernateCompatabilityTests.Mocks
{
	public class NHibernateMappingAssemblyContainer : BaseMappingAssemblyContainer
	{
		public NHibernateMappingAssemblyContainer()
		{
			AddAssemblyContainingType<NHibernateMappingAssemblyContainer>();
		}
	}
}
