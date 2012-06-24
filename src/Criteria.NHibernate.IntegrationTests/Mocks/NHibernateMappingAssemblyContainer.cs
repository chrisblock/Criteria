using Criteria.Sql.Impl;

namespace Criteria.NHibernate.IntegrationTests.Mocks
{
	public class NHibernateMappingAssemblyContainer : BaseMappingAssemblyContainer
	{
		public NHibernateMappingAssemblyContainer()
		{
			AddAssemblyContainingType<NHibernateMappingAssemblyContainer>();
		}
	}
}
