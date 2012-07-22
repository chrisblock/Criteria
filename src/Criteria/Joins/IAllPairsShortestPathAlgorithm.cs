using System.Collections.Generic;

using Criteria.Registries;

namespace Criteria.Joins
{
	public interface IAllPairsShortestPathAlgorithm
	{
		AllPairsShortestPathResult Solve(IEnumerable<JoinPath> joinPaths);
	}
}
