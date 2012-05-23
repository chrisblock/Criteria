using StructureMap;

namespace Criteria.Tests
{
	public static class StructureMapBootstrapper
	{
		public static void Bootstrap()
		{
			ObjectFactory.Initialize(init => init.AddRegistry<CriteriaRegistry>());
		}
	}
}
