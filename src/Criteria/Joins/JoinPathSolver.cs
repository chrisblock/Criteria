using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Criteria.Expressions;
using Criteria.Json;
using Criteria.Registries;
using Criteria.Registries.Impl;

using Iesi.Collections.Generic;

namespace Criteria.Joins
{
	public class JoinPathSolver
	{
		private readonly JoinConfiguration _joinConfiguration;
		private readonly ExpressionBuilder _expressionBuilder;
		private readonly FindParameterTypesVisitor _findNecessaryTypesVisitor;

		private readonly Iesi.Collections.Generic.ISet<JoinPath> _joinPaths;
		private readonly IList<Type> _joinedTypes;
		private readonly IDictionary<Type, int> _typeIndexLookup;
		private readonly int[][] _adjacencyMatrix;
		private readonly int[][] _distanceMatrix;
		private readonly int[][] _nextHopMatrix;

		private JoinPathSolver(JoinConfiguration joinConfiguration)
		{
			_joinConfiguration = joinConfiguration;
			_expressionBuilder = joinConfiguration.ExpressionBuilder;
			_findNecessaryTypesVisitor = new FindParameterTypesVisitor();

			_joinPaths = new HashedSet<JoinPath>(joinConfiguration.JoinPathRegistry.JoinPaths);

			_joinedTypes = _joinPaths.Select(x => x.End)
				.Distinct()
				.ToList();

			_typeIndexLookup = _joinedTypes.ToDictionary(k => k, v => _joinedTypes.IndexOf(v));

			_adjacencyMatrix = BaseJoinPathRegistry.BuildAdjacencyMatrix(_joinPaths);

			var floydWarshallResult = BaseJoinPathRegistry.FloydWarshallAlgorithm(_adjacencyMatrix);

			_distanceMatrix = floydWarshallResult.Item1;

			_nextHopMatrix = floydWarshallResult.Item2;
		}

		public static JoinPathSolver With(JoinConfiguration joinConfiguration)
		{
			return new JoinPathSolver(joinConfiguration);
		}

		public ConstrainedJoinPart SolveFor<TStart>(JsonCriteriaNode criteria)
		{
			var expression = _expressionBuilder.Build(criteria);

			var requiredTypes = GetRequiredTypes<TStart>(expression).ToList();

			var missingTypes = requiredTypes
				.Where(x => (_joinedTypes.Contains(x.Key) == false) && (x.Key != typeof(TStart)))
				.ToList();

			if(missingTypes.Any())
			{
				throw new ArgumentException(String.Format("Cannot constrain types [{0}]. Those types haven't been configured in the JoinPathRegistry.", String.Join(", ", missingTypes)));
			}

			var join = Join.Using(_joinConfiguration)
				.StartWith<TStart>();

			ConstrainedJoinPart result;

			if (_joinPaths.Any())
			{
				var joinPaths = ConstructJoinPathList(typeof (TStart), requiredTypes);

				var joinPathAggregate = joinPaths.Aggregate(join, (current, typeDependency) => typeDependency.ApplyJoin(current));

				result = joinPathAggregate.Where(criteria);
			}
			else
			{
				result = join.Where(criteria);
			}

			return result;
		}

		private IEnumerable<JoinPath> ConstructJoinPathList(Type startingType, IEnumerable<KeyValuePair<Type, int>> typeCounts)
		{
			var joinPaths = new List<IEnumerable<JoinPath>>();

			var result = new List<JoinPath>();

			var pathsToTypes = typeCounts
				.ToDictionary(key => key.Key, value => GetJoinPath(startingType, value.Key));

			foreach (var type in typeCounts)
			{
				if (pathsToTypes[type.Key].Any() == false)
				{
					continue;
				}
				if (pathsToTypes[type.Key].Any(x => x.IsOneToMany))
				{
					for(var c = 0; c < type.Value; c++)
					{
						joinPaths.Add(pathsToTypes[type.Key]);
					}
				}
				else
				{
					joinPaths.Add(pathsToTypes[type.Key]);
				}
			}

			foreach(var joinPathEnumerable in joinPaths)
			{
				foreach(var path in joinPathEnumerable)
				{
					if (result.Contains(path) && !path.IsOneToMany)
					{
						continue;
					}

					result.Add(path);
				}
			}

			return result;
		}

		private IEnumerable<KeyValuePair<Type, int>> GetRequiredTypes<TResult>(Expression expression)
		{
			var resultType = typeof (TResult);

			var requiredTypes = _findNecessaryTypesVisitor
				.FindParameterTypes(expression);

			if(requiredTypes.ContainsKey(resultType) == false)
			{
				requiredTypes.Add(resultType, 1);
			}

			return requiredTypes;
		}

		private IEnumerable<JoinPath> GetJoinPath(Type start, Type end)
		{
			var i = _typeIndexLookup[start];
			var j = _typeIndexLookup[end];

			if(_distanceMatrix[i][j] == int.MaxValue)
			{
				throw new ArgumentException(String.Format("No path found from type \"{0}\" to type \"{1}\".", start, end));
			}

			var intermediate = _nextHopMatrix[i][j];
			var intermediatePath = _joinPaths.Where(x => (x.Start == start) && (x.End == end));

			IEnumerable<JoinPath> result;

			if(intermediate == -1)
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
