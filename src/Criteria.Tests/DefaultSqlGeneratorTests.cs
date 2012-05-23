using System;

using Criteria.Expressions;
using Criteria.Joins;
using Criteria.Registries;
using Criteria.Tests.LinqToObjectsModel;
using Criteria.Tests.Mocks;

using NUnit.Framework;

using StructureMap;

namespace Criteria.Tests
{
	[TestFixture]
	public class DefaultSqlGeneratorTests
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
				QueryableProvider = new LinqToObjectsQueryableProvider()
			};
		}

		[Test]
		public void Generate_WithExpression_ThrowsException()
		{
			Assert.That(
				() => Join.Using(_joinConfiguration)
					.StartWith<LinqToObjectsOne>()
					.Sql<LinqToObjectsOne>(),
				Throws.InstanceOf<NotImplementedException>());
		}

		[Test]
		public void GenerateUnaliased_WithExpression_ThrowsException()
		{
			Assert.That(
				() => Join.Using(_joinConfiguration)
					.StartWith<LinqToObjectsOne>()
					.Sql<LinqToObjectsOne>(x => x.Unalias()),
				Throws.InstanceOf<NotImplementedException>());
		}

		[Test]
		public void GenerateStarSelect_WithExpression_ThrowsException()
		{
			Assert.That(
				() => Join.Using(_joinConfiguration)
					.StartWith<LinqToObjectsOne>()
					.Sql<LinqToObjectsOne>(x => x.StarSelect()),
				Throws.InstanceOf<NotImplementedException>());
		}
	}
}
