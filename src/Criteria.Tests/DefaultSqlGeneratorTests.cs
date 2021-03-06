﻿// ReSharper disable InconsistentNaming

using System;

using Criteria.Expressions;
using Criteria.Joins;
using Criteria.Sql;
using Criteria.Sql.Impl;
using Criteria.Tests.LinqToObjectsModel;
using Criteria.Tests.Mocks;

using NUnit.Framework;

namespace Criteria.Tests
{
	[TestFixture]
	public class DefaultSqlGeneratorTests
	{
		private JoinConfiguration _joinConfiguration;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_joinConfiguration = new JoinConfiguration
			{
				ExpressionBuilder = new ExpressionBuilder(new MockCriteriaTypeRegistry()),
				QueryableProvider = new LinqToObjectsQueryableProvider()
			};
		}

		[Test]
		public void Generate_WithExpression_ThrowsException()
		{
			var configuration = new SqlGeneratorConfiguration();

			Assert.That(
				() => Join.Using(_joinConfiguration)
					.StartWith<LinqToObjectsOne>()
					.Sql<LinqToObjectsOne>(new SqlGenerator(), configuration),
				Throws.InstanceOf<NotImplementedException>());
		}

		[Test]
		public void GenerateUnaliased_WithExpression_ThrowsException()
		{
			var configuration = new SqlGeneratorConfiguration
			{
				Unalias = true
			};

			Assert.That(
				() => Join.Using(_joinConfiguration)
					.StartWith<LinqToObjectsOne>()
					.Sql<LinqToObjectsOne>(new SqlGenerator(), configuration),
				Throws.InstanceOf<NotImplementedException>());
		}

		[Test]
		public void GenerateStarSelect_WithExpression_ThrowsException()
		{
			var configuration = new SqlGeneratorConfiguration
			{
				StarSelect = true
			};

			Assert.That(
				() => Join.Using(_joinConfiguration)
					.StartWith<LinqToObjectsOne>()
					.Sql<LinqToObjectsOne>(new SqlGenerator(), configuration),
				Throws.InstanceOf<NotImplementedException>());
		}
	}
}
