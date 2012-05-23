using Criteria.Json;

namespace Criteria.Registries
{
	public interface ICriteriaTypeRegistry
	{
		TypeRegistryResult Lookup(string key);
		TypeRegistryResult Lookup(ICriteriaLeaf leaf);
	}
}
