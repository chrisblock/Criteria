using System.Collections.Generic;
using System.Reflection;

namespace Criteria.Sql
{
	public interface IMappingAssemblyContainer
	{
		IEnumerable<Assembly> GetMappingAssemblies();
	}
}
