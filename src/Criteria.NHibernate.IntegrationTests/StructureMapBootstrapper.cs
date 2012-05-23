using Criteria.NHibernate;

using StructureMap;

namespace Criteria.NHibernateCompatabilityTests
{
	public static class StructureMapBootstrapper
	{
		public static void Bootstrap()
		{
			ObjectFactory.Initialize(init => init.AddRegistry<CriteriaNHibernateRegistry>());
		}
	}
}
