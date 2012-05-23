using System;
using System.Collections.Generic;
using System.Linq;

namespace Criteria.Registries.Impl
{
	public class BaseJoinPathRegistry : IJoinPathRegistry
	{
		//TODO :Extract out floyd into Into here or seperate class

		public ICollection<JoinPath> JoinPaths { get; private set; }
		public Dictionary<Type, bool> MultipleJoinLookup { get; set; }

		public BaseJoinPathRegistry()
		{
			JoinPaths = new List<JoinPath>();
			MultipleJoinLookup = new Dictionary<Type, bool>();
		}

		protected void RegisterOneTimeJoinFor<T>(Action<JoinPathConfigurator<T>> configureJoinConfigurator)
		{
			var configurator = new JoinPathConfigurator<T>(JoinPaths);

			configureJoinConfigurator(configurator);
		}

		private void AddToDictionary(Type T)
		{
			if (!MultipleJoinLookup.ContainsKey(T))
			{
				MultipleJoinLookup.Add(T, true);
			}
		}

		protected void RegisterOneToManyJoinFor<T>(Func<JoinPathConfigurator<T>, JoinPathToConfigurator> configureJoinConfigurator)
		{
			var configurator = new JoinPathConfigurator<T>(JoinPaths, true);

			var result = configureJoinConfigurator(configurator);
			
			AddToDictionary(result.OuterType);
		}

		public static Tuple<int[][], int[][]> FloydWarshallAlgorithm(int[][] adjacencyMatrix)
		{
			var n = adjacencyMatrix.Length;
			var floydWarshallResult = adjacencyMatrix;
			var next = BuildSquareArray<int>(n, x => y => -1);

			for (var k = 0; k < n; k++)
			{
				for (var i = 0; i < n; i++)
				{
					for (var j = 0; j < n; j++)
					{
						var sum = ((floydWarshallResult[i][k] == int.MaxValue) || (floydWarshallResult[k][j] == int.MaxValue))
									? (int.MaxValue)
									: (floydWarshallResult[i][k] + floydWarshallResult[k][j]);

						if (sum < floydWarshallResult[i][j])
						{
							floydWarshallResult[i][j] = sum;
							next[i][j] = k;
						}
					}
				}
			}

			return new Tuple<int[][], int[][]>(floydWarshallResult, next);
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

		public static int[][] BuildAdjacencyMatrix(Iesi.Collections.Generic.ISet<JoinPath> joinPaths)
		{
			var joinedTypes = joinPaths.Select(x => x.End)
				.Distinct()
				.ToList();
			var n = joinedTypes.Count;

			var typeIndexLookup = joinedTypes.ToDictionary(k => k, joinedTypes.IndexOf);

			var adjacencyMatrix = BuildSquareArray<int>(n, x => y => (x == y) ? 0 : int.MaxValue);


			foreach (var joinPath in joinPaths)
			{
				var x = typeIndexLookup[joinPath.End];
				var y = typeIndexLookup[joinPath.Start];
				adjacencyMatrix[x][y] = 1;
			}

			return adjacencyMatrix;
		}
	}
}
