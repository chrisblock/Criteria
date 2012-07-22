using System;
using System.Collections.Generic;
using System.Linq;

using Criteria.Registries;

namespace Criteria.Joins.Impl
{
	public class FloydWarshallAlgorithm : IAllPairsShortestPathAlgorithm
	{
		public AllPairsShortestPathResult Solve(IEnumerable<JoinPath> joinPaths)
		{
			var joinedTypes = joinPaths.Select(x => x.End)
				.Distinct()
				.ToList();

			var typeIndexLookup = joinedTypes.ToDictionary(k => k, joinedTypes.IndexOf);

			var distanceMatrix = BuildAdjacencyMatrix(typeIndexLookup, joinPaths);
			var n = distanceMatrix.Length;
			
			var nextHopMatrix = BuildSquareArray<int>(n, x => y => -1);

			for (var k = 0; k < n; k++)
			{
				for (var i = 0; i < n; i++)
				{
					for (var j = 0; j < n; j++)
					{
						var sum = ((distanceMatrix[i][k] == int.MaxValue) || (distanceMatrix[k][j] == int.MaxValue))
							? (int.MaxValue)
							: (distanceMatrix[i][k] + distanceMatrix[k][j]);

						if (sum < distanceMatrix[i][j])
						{
							distanceMatrix[i][j] = sum;
							nextHopMatrix[i][j] = k;
						}
					}
				}
			}

			return new FloydWarshallResult(typeIndexLookup, joinedTypes, joinPaths, distanceMatrix, nextHopMatrix);
		}

		private static int [][] BuildAdjacencyMatrix(IDictionary<Type, int> typeIndexLookup, IEnumerable<JoinPath> joinPaths)
		{
			var n = typeIndexLookup.Count;

			var adjacencyMatrix = BuildSquareArray<int>(n, x => y => (x == y) ? 0 : int.MaxValue);

			foreach (var joinPath in joinPaths)
			{
				var x = typeIndexLookup[joinPath.End];
				var y = typeIndexLookup[joinPath.Start];
				adjacencyMatrix[x][y] = 1;
			}

			return adjacencyMatrix;
		}

		private static T[][] BuildSquareArray<T>(int size, Func<int, Func<int, T>> initialValue)
		{
			return BuildTwoDimensionalArray(size, size, initialValue);
		}

		private static T[][] BuildTwoDimensionalArray<T>(int x, int y, Func<int, Func<int, T>> initialValue)
		{
			var m = BuildArray(x, i => BuildArray(y, initialValue(i)));

			return m;
		}

		private static T[] BuildArray<T>(int size, Func<int, T> initialValue)
		{
			var array = new T[size];

			for (var i = 0; i < size; i++)
			{
				array[i] = initialValue(i);
			}

			return array;
		}
	}
}
