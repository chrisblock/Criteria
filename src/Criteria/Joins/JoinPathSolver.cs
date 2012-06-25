using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Criteria.Expressions;
using Criteria.Joins.Impl;
using Criteria.Json;
using Criteria.Registries;

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
		private readonly AllPairsShortestPathResult _allPairsShortestPathResult;

		private JoinPathSolver(IAllPairsShortestPathAlgorithm allPairsShortestPathAlgorithm, JoinConfiguration joinConfiguration)
		{
			_joinConfiguration = joinConfiguration;
			_expressionBuilder = joinConfiguration.ExpressionBuilder;
			_findNecessaryTypesVisitor = new FindParameterTypesVisitor();

			_joinPaths = new HashedSet<JoinPath>(joinConfiguration.JoinPathRegistry.JoinPaths);

			_joinedTypes = _joinPaths.Select(x => x.End)
				.Distinct()
				.ToList();

			_allPairsShortestPathResult = allPairsShortestPathAlgorithm.Solve(_joinPaths);
		}

		public static JoinPathSolver With(JoinConfiguration joinConfiguration)
		{
			return new JoinPathSolver(new FloydWarshallAlgorithm(), joinConfiguration);
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
				.ToDictionary(key => key.Key, value => _allPairsShortestPathResult.GetJoinPath(startingType, value.Key));

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
					if (result.Contains(path) && (path.IsOneToMany == false))
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
	}
}
