using System.Collections.Generic;

using Criteria.Registries;
using Criteria.Registries.Impl;

namespace Criteria.Joins
{
	public interface IAllPairsShortestPathAlgorithm
	{
		AllPairsShortestPathResult Solve(IEnumerable<JoinPath> joinPaths);
	}
}
