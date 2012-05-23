using StructureMap.Configuration.DSL;

namespace Criteria
{
	public class CriteriaRegistry : Registry
	{
		public CriteriaRegistry()
		{
			Scan(scan =>
			{
				scan.AssemblyContainingType<CriteriaRegistry>();
				scan.WithDefaultConventions();
			});
		}
	}
}
