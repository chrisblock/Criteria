using StructureMap;

namespace Criteria.NHibernate.IntegrationTests
{
	public static class StructureMapBootstrapper
	{
		public static void Bootstrap()
		{
			ObjectFactory.Initialize(init => init.AddRegistry<CriteriaNHibernateRegistry>());
		}
	}
}
