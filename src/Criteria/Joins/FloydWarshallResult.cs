using System;
using System.Collections.Generic;
using System.Linq;

using Criteria.Registries;

namespace Criteria.Joins
{
	public class FloydWarshallResult : AllPairsShortestPathResult
	{
		private readonly Dictionary<Type, int> _typeIndexLookup;
		private readonly IList<Type> _joinedTypes;
		private readonly IEnumerable<JoinPath> _joinPaths;
		private readonly int[][] _distanceMatrix;
		private readonly int[][] _nextHopMatrix;

		public FloydWarshallResult(Dictionary<Type, int> typeIndexLookup, IList<Type> joinedTypes, IEnumerable<JoinPath> joinPaths, int[][] distanceMatrix, int[][] nextHopMatrix)
		{
			_typeIndexLookup = typeIndexLookup;
			_joinedTypes = joinedTypes;
			_joinPaths = joinPaths;
			_distanceMatrix = distanceMatrix;
			_nextHopMatrix = nextHopMatrix;
		}

		public override IEnumerable<JoinPath> GetJoinPath(Type start, Type end)
		{
			var i = _typeIndexLookup[start];
			var j = _typeIndexLookup[end];

			if (_distanceMatrix[i][j] == int.MaxValue)
			{
				throw new ArgumentException(String.Format("No path found from type \"{0}\" to type \"{1}\".", start, end));
			}

			var intermediate = _nextHopMatrix[i][j];
			var intermediatePath = _joinPaths.Where(x => (x.Start == start) && (x.End == end));

			IEnumerable<JoinPath> result;

			if (intermediate == -1)
			{
				result = intermediatePath;
			}
			else
			{
				var intermediateType = _joinedTypes[intermediate];

				var pathToIntermediate = GetJoinPath(start, intermediateType);
				var pathFromIntermediate = GetJoinPath(intermediateType, end);

				result = pathToIntermediate.Concat(pathFromIntermediate);
			}

			return result;
		}
	}
}
