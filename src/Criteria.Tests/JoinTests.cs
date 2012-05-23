using System;
using System.Collections.Generic;
using System.Linq;

using Criteria.Expressions;
using Criteria.Joins;
using Criteria.Json;
using Criteria.Registries;
using Criteria.Registries.Impl;
using Criteria.Tests.LinqToObjectsModel;
using Criteria.Tests.Mocks;

using NUnit.Framework;

using StructureMap;

namespace Criteria.Tests
{
	[TestFixture]
	public class JoinTests
	{
		private JoinConfiguration _joinConfiguration;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			StructureMapBootstrapper.Bootstrap();

			ObjectFactory.EjectAllInstancesOf<ICriteriaTypeRegistry>();

			ObjectFactory.Inject<ICriteriaTypeRegistry>(new MockCriteriaTypeRegistry());

			_joinConfiguration = new JoinConfiguration
			{
				ExpressionBuilder = new ExpressionBuilder(new MockCriteriaTypeRegistry()),
				QueryableProvider = new LinqToObjectsQueryableProvider(),
				JoinPathRegistry = new BaseJoinPathRegistry
				{
					MultipleJoinLookup = new Dictionary<Type, bool>()
				}
			};
		}

		[Test]
		public void Join_NoJoinsBuildsCorrectly()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<LinqToObjectsOne>()
				.Query<LinqToObjectsOne>()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(6));
		}

		[Test]
		public void Join_NoJoinsWithWhereBuildsCorrectly()
		{
			var criteria = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Key = "LinqToObjectsOneField",
						Operator = Operator.Equal,
						Value = "OneOne"
					}
				}
			};

			var result = Join.Using(_joinConfiguration)
				.StartWith<LinqToObjectsOne>()
				.Where(criteria)
				.Query<LinqToObjectsOne>()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Join_NoJoinsWithWhereExpressionBuildsCorrectly()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<LinqToObjectsOne>()
				.Where<LinqToObjectsOne>(x => x.LinqToObjectsOneField == "OneOne")
				.Query<LinqToObjectsOne>()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Join_NoJoinsWithSupplementalExpressionWhereBuildsCorrectly()
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

			var result = Join.Using(_joinConfiguration)
				.StartWith<LinqToObjectsOne>()
				.Where(criteria)
				.Where<LinqToObjectsOne>(x => x.LinqToObjectsOneField != "OneTwo")
				.Query<LinqToObjectsOne>()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(4));
		}

		[Test]
		public void Join_NoJoinsWithSupplementalCriteriaWhereBuildsCorrectly()
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

			var additionalCriteria = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Key = "LinqToObjectsOneField",
						Operator = Operator.NotEqual,
						Value = "OneTwo"
					}
				}
			};

			var result = Join.Using(_joinConfiguration)
				.StartWith<LinqToObjectsOne>()
				.Where(criteria)
				.Where(additionalCriteria)
				.Query<LinqToObjectsOne>()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(4));
		}

		[Test]
		public void Join_BuildsJoinExpressionCorrectly()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<LinqToObjectsOne>()
				.Join<LinqToObjectsOne>().To<LinqToObjectsTwo>().On(p => p.LinqToObjectsOneId, c => c.LinqToObjectsOneParentId)
				.Query<LinqToObjectsOne>()
				.Distinct()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(3));
		}

		[Test]
		public void Join_JoinToTheSameType_ThrowsException()
		{
			Assert.That(() =>
			{
				Join.Using(_joinConfiguration)
					.StartWith<LinqToObjectsOne>()
					.Join<LinqToObjectsOne>().To<LinqToObjectsOne>();
			}, Throws.ArgumentException);
		}

		[Test]
		public void Join_BuildsJoinExpressionWithJoinToNonRootEntitiesCorrectly()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<LinqToObjectsOne>()
				.Join<LinqToObjectsOne>().To<LinqToObjectsTwo>().On(p => p.LinqToObjectsOneId, c => c.LinqToObjectsOneParentId)
				.Join<LinqToObjectsTwo>().To<LinqToObjectsThree>().On(o => o.LinqToObjectsOneParentId, t => t.LinqToObjectsOneParentId)
				.Query<LinqToObjectsOne>()
				.Distinct()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Join_BuildsJoinExpressionWithWhereCriteriaCorrectly()
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
						Value = "OneTwo"
					}
				}
			};

			var result = Join.Using(_joinConfiguration)
				.StartWith<LinqToObjectsOne>()
				.Join<LinqToObjectsOne>().To<LinqToObjectsTwo>().On(p => p.LinqToObjectsOneId, c => c.LinqToObjectsOneParentId)
				.Join<LinqToObjectsOne>().To<LinqToObjectsThree>().On(p => p.LinqToObjectsOneId, c => c.LinqToObjectsOneParentId)
				.Where(criteria)
				.Query<LinqToObjectsOne>()
				.Distinct()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Join_BuildsJoinExpressionWithWhereExpressionCorrectly()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<LinqToObjectsOne>()
				.Join<LinqToObjectsOne>().To<LinqToObjectsTwo>().On(p => p.LinqToObjectsOneId, c => c.LinqToObjectsOneParentId)
				.Join<LinqToObjectsOne>().To<LinqToObjectsThree>().On(p => p.LinqToObjectsOneId, c => c.LinqToObjectsOneParentId)
				.Where<LinqToObjectsOne>(x => x.LinqToObjectsOneField != "OneTwo")
				.Query<LinqToObjectsOne>()
				.Distinct()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Join_BuildsJoinExpressionWithAdditionalExpressionCriteriaWhereCorrectly()
		{
			var criteria = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Key = "LinqToObjectsOneField",
						Operator = Operator.Equal,
						Value = "OneOne"
					}
				}
			};

			var result = Join.Using(_joinConfiguration)
				.StartWith<LinqToObjectsOne>()
				.Join<LinqToObjectsOne>().To<LinqToObjectsTwo>().On(p => p.LinqToObjectsOneId, c => c.LinqToObjectsOneParentId)
				.Join<LinqToObjectsOne>().To<LinqToObjectsThree>().On(p => p.LinqToObjectsOneId, t => t.LinqToObjectsOneParentId)
				.Where(criteria)
				.Where<LinqToObjectsTwo>(x => x.LinqToObjectsTwoField == "TwoTwo")
				.Query<LinqToObjectsOne>()
				.Distinct()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Join_BuildsJoinExpressionWithAdditionalJsonObjectCriteriaWhereCorrectly()
		{
			var criteria = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Key = "LinqToObjectsOneField",
						Operator = Operator.Equal,
						Value = "OneOne"
					}
				}
			};

			var additionalCriteria = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Key = "LinqToObjectsTwoField",
						Operator = Operator.Equal,
						Value = "TwoTwo"
					}
				}
			};

			var result = Join.Using(_joinConfiguration)
				.StartWith<LinqToObjectsOne>()
				.Join<LinqToObjectsOne>().To<LinqToObjectsTwo>().On(p => p.LinqToObjectsOneId, c => c.LinqToObjectsOneParentId)
				.Join<LinqToObjectsOne>().To<LinqToObjectsThree>().On(p => p.LinqToObjectsOneId, t => t.LinqToObjectsOneParentId)
				.Where(criteria)
				.Where(additionalCriteria)
				.Query<LinqToObjectsOne>()
				.Distinct()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Project_PopulatesResultTypeCorrectly()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<LinqToObjectsOne>()
				.Join<LinqToObjectsOne>().To<LinqToObjectsTwo>().On(p => p.LinqToObjectsOneId, c => c.LinqToObjectsOneParentId)
				.Join<LinqToObjectsOne>().To<LinqToObjectsThree>().On(p => p.LinqToObjectsOneId, t => t.LinqToObjectsOneParentId)
				.Project<LinqToObjectsComposite>()
				.ToList();

			Assert.That(result, Is.Not.Empty);
			Assert.That(result.Select(x => x.One).Count(), Is.EqualTo(result.Count));
			Assert.That(result.Select(x => x.Two).Count(), Is.EqualTo(result.Count));
		}
	}
}
