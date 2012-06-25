// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using System.Linq;

using Criteria.Expressions;
using Criteria.Joins;
using Criteria.Json;
using Criteria.Tests.LinqToObjectsModel;
using Criteria.Tests.Mocks;

using NUnit.Framework;

namespace Criteria.Tests
{
	[TestFixture]
	public class JoinPathSolverTests
	{
		private JoinPathSolver _joinPathSolver;

		[SetUp]
		public void TestSetUp()
		{
			_joinPathSolver = JoinPathSolver.With(new JoinConfiguration
			{
				ExpressionBuilder = new ExpressionBuilder(new MockCriteriaTypeRegistry()),
				QueryableProvider = new LinqToObjectsQueryableProvider(),
				JoinPathRegistry = new MockJoinPathRegistry()
			});
		}

		[Test]
		public void SolveFor_TypeNotRegisteredInJoinPathRegistry_ThrowsException()
		{
			var criteria = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Key = "UnregisteredLinqToObjects",
						Operator = Operator.NotEqual,
						Value = "OneOne"
					}
				}
			};

			Assert.That(() =>
			{
				_joinPathSolver
					.SolveFor<LinqToObjectsOne>(criteria);
			}, Throws.ArgumentException);
		}

		[Test]
		public void SolveFor_MissingTypeRegistryInJoinPathRegistry_ThrowsMissingTypeException()
		{
			var criteria = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Key = "TestStringProperty",
						Operator = Operator.NotEqual,
						Value = "OneOne"
					}
				}
			};

			Assert.That(() =>
			{
				_joinPathSolver
					.SolveFor<LinqToObjectsOne>(criteria);
			}, Throws.ArgumentException);
		}

		[Test]
		public void SolveFor_EmptyJoinPathRegistryAndNoJoins_CorrectlyQueryies()
		{
			_joinPathSolver = JoinPathSolver.With(new JoinConfiguration
			{
				ExpressionBuilder = new ExpressionBuilder(new MockCriteriaTypeRegistry()),
				QueryableProvider = new LinqToObjectsQueryableProvider(),
				JoinPathRegistry = new EmptyJoinPathRegistry()
			});

			var criteria = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Key = "LinqToObjectsOneField",
						Operator = Operator.NotEqual,
						Value = "OneOne"
					}
				}
			};

			var result = _joinPathSolver
				.SolveFor<LinqToObjectsOne>(criteria)
				.Query<LinqToObjectsOne>()
				.Distinct()
				.ToList();

			Assert.That(result.Count, Is.EqualTo(5));
		}

		[Test]
		public void SolveFor_SingleType_CorrectlyQueries()
		{
			var criteria = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Key = "LinqToObjectsOneField",
						Operator = Operator.NotEqual,
						Value = "OneOne"
					}
				}
			};

			var result = _joinPathSolver
				.SolveFor<LinqToObjectsOne>(criteria)
				.Query<LinqToObjectsOne>()
				.Distinct()
				.ToList();

			Assert.That(result.Count, Is.EqualTo(5));
		}

		[Test]
		public void SolveFor_TwoTypes_CorrectlyQueries()
		{
			var criteria = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Key = "LinqToObjectsOneField",
						Operator = Operator.NotEqual,
						Value = "OneOne"
					}
				}
			};

			var result = _joinPathSolver
				.SolveFor<LinqToObjectsTwo>(criteria)
				.Query<LinqToObjectsTwo>()
				.Distinct()
				.ToList();

			Assert.That(result.Count, Is.EqualTo(4));
		}

		[Test]
		public void SolveFor_TwoDisjointTypes_CorrectlyQueries()
		{
			var criteria = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Key = "LinqToObjectsTwoField",
						Operator = Operator.NotEqual,
						Value = "TwoOne"
					},
					new JsonCriteriaNode
					{
						Key = "LinqToObjectsThreeField",
						Operator = Operator.NotEqual,
						Value = "ThreeTwo"
					}
				}
			};

			var result = _joinPathSolver
				.SolveFor<LinqToObjectsTwo>(criteria)
				.Query<LinqToObjectsTwo>()
				.Distinct()
				.ToList();

			Assert.That(result.Count, Is.EqualTo(1));
		}
	}
}
