using System;
using System.Collections.Generic;

namespace Criteria.Registries
{
	public interface IJoinPathRegistry
	{
		ICollection<JoinPath> JoinPaths { get; }
		Dictionary<Type, bool> MultipleJoinLookup { get; }
	}
}
