using System;
using System.Collections.Generic;

using Criteria.Registries;

namespace Criteria.Joins
{
	public abstract class AllPairsShortestPathResult
	{
		public abstract IEnumerable<JoinPath> GetJoinPath(Type start, Type end);
	}
}
