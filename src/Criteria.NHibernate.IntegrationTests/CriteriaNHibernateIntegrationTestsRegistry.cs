using StructureMap.Configuration.DSL;

namespace Criteria.NHibernate.IntegrationTests
{
	public class CriteriaNHibernateIntegrationTestsRegistry : Registry
	{
		public CriteriaNHibernateIntegrationTestsRegistry()
		{
			Scan(scan =>
			{
				scan.AssemblyContainingType<CriteriaNHibernateIntegrationTestsRegistry>();

				scan.WithDefaultConventions();
			});

			IncludeRegistry<CriteriaNHibernateRegistry>();
		}
	}
}
