using System;
using System.Collections.Generic;
using System.Linq;

using Criteria.Expressions;
using Criteria.Joins;
using Criteria.NHibernate;
using Criteria.NHibernateCompatabilityTests.App_Start;
using Criteria.NHibernateCompatabilityTests.Mocks;
using Criteria.NHibernateCompatabilityTests.SqlLite;
using Criteria.NHibernateCompatabilityTests.TreeModel;
using Criteria.Registries;
using Criteria.Registries.Impl;
using Criteria.Sql;

using NHibernate;

using NUnit.Framework;

using StructureMap;

namespace Criteria.NHibernateCompatabilityTests
{
	[TestFixture]
	public class SqlGenerationTests
	{
		private ISession _session;
		private SqlLiteBuilder _sqlLiteBuilder;
		private JoinConfiguration _joinConfiguration;

		private void PerformSessionAction(Action<ISession> sessionAction)
		{
			using (var transaction = _session.BeginTransaction())
			{
				try
				{
					sessionAction(_session);
					transaction.Commit();
				}
				catch (Exception)
				{
					if (transaction.WasRolledBack == false)
					{
						transaction.Rollback();
					}

					throw;
				}
			}
		}

		private void InsertTestData()
		{
			PerformSessionAction(session =>
			{
				session.Save(new RootEntity
				{
					RootEntityField = "RootEntityFieldValue",
					OneLevelEntities = new List<OneLevelEntity>
					{
						new OneLevelEntity
						{
							OneLevelEntityField = "OneLevelEntityFieldValue"
						},
						new OneLevelEntity
						{
							OneLevelEntityField = "AnotherOneLevelEntityFieldValue"
						}
					},
					TwoLevelEntities = new List<TwoLevelEntity>
					{
						new TwoLevelEntity
						{
							TwoLevelEntityField = "TwoLevelEntityFieldValue",
							TwoLevelEntityChildren = new List<TwoLevelEntityChild>
							{
								new TwoLevelEntityChild
								{
									TwoLevelEntityChildField = "TwoLevelEntityChildFieldValue"
								},
								new TwoLevelEntityChild
								{
									TwoLevelEntityChildField = "AnotherTwoLevelEntityChildFieldValue"
								}
							}
						},
						new TwoLevelEntity
						{
							TwoLevelEntityField = "AnotherTwoLevelEntityFieldValue",
							TwoLevelEntityChildren = new List<TwoLevelEntityChild>
							{
								new TwoLevelEntityChild
								{
									TwoLevelEntityChildField = "YetAnotherTwoLevelEntityChildFieldValue"
								},
								new TwoLevelEntityChild
								{
									TwoLevelEntityChildField = "StillAnotherTwoLevelEntityChildFieldValue"
								}
							}
						}
					},
					ThreeLevelEntities = new List<ThreeLevelEntity>
					{
						new ThreeLevelEntity
						{
							ThreeLevelEntityField = "ThreeLevelEntityFieldValue",
							ThreeLevelEntityChildren = new List<ThreeLevelEntityChild>
							{
								new ThreeLevelEntityChild
								{
									ThreeLevelEntityChildField = "ThreeLevelEntityChildFieldValue",
									ThreeLevelEntityGrandchildren = new List<ThreeLevelEntityGrandchild>
									{
										new ThreeLevelEntityGrandchild
										{
											ThreeLevelEntityGrandchildField = "ThreeLevelEntityGrandchildFieldValue"
										},
										new ThreeLevelEntityGrandchild
										{
											ThreeLevelEntityGrandchildField = "AnotherThreeLevelEntityGrandchildFieldValue"
										},
									}
								},
								new ThreeLevelEntityChild
								{
									ThreeLevelEntityChildField = "AnotherThreeLevelEntityChildFieldValue",
									ThreeLevelEntityGrandchildren = new List<ThreeLevelEntityGrandchild>
									{
										new ThreeLevelEntityGrandchild
										{
											ThreeLevelEntityGrandchildField = "YetAnotherThreeLevelEntityGrandchildFieldValue"
										},
										new ThreeLevelEntityGrandchild
										{
											ThreeLevelEntityGrandchildField = "StillAnotherThreeLevelEntityGrandchildFieldValue"
										},
									}
								},
							}
						},
						new ThreeLevelEntity
						{
							ThreeLevelEntityField = "AnotherThreeLevelEntityFieldValue",
							ThreeLevelEntityChildren = new List<ThreeLevelEntityChild>
							{
								new ThreeLevelEntityChild
								{
									ThreeLevelEntityChildField = "YetAnotherThreeLevelEntityChildFieldValue",
									ThreeLevelEntityGrandchildren = new List<ThreeLevelEntityGrandchild>
									{
										new ThreeLevelEntityGrandchild
										{
											ThreeLevelEntityGrandchildField = "DifferentThreeLevelEntityGrandchildFieldValue"
										},
										new ThreeLevelEntityGrandchild
										{
											ThreeLevelEntityGrandchildField = "AnotherDifferentThreeLevelEntityGrandchildFieldValue"
										},
									}
								},
								new ThreeLevelEntityChild
								{
									ThreeLevelEntityChildField = "StillAnotherThreeLevelEntityChildFieldValue",
									ThreeLevelEntityGrandchildren = new List<ThreeLevelEntityGrandchild>
									{
										new ThreeLevelEntityGrandchild
										{
											ThreeLevelEntityGrandchildField = "YetAnotherDifferentThreeLevelEntityGrandchildFieldValue"
										},
										new ThreeLevelEntityGrandchild
										{
											ThreeLevelEntityGrandchildField = "StillAnotherDifferentThreeLevelEntityGrandchildFieldValue"
										},
									}
								},
							}
						}
					}
				});
			});
		}

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			StructureMapBootstrapper.Bootstrap();

			NHibernateProfilerBootstrapper.PreStart();

			ObjectFactory.EjectAllInstancesOf<ICriteriaTypeRegistry>();

			ObjectFactory.Inject<ICriteriaTypeRegistry>(new TreeModelCriteriaTypeRegistry());

			ObjectFactory.Inject<IMappingAssemblyContainer>(new NHibernateMappingAssemblyContainer());
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			NHibernateProfilerBootstrapper.PostStop();
		}

		[SetUp]
		public void TestSetUp()
		{
			_sqlLiteBuilder = new SqlLiteBuilder();
			_session = _sqlLiteBuilder.GetSession();

			InsertTestData();

			_session.Flush();

			_session.Clear();

			_joinConfiguration = new JoinConfiguration
			{
				ExpressionBuilder = new ExpressionBuilder(new TreeModelCriteriaTypeRegistry()),
				QueryableProvider = new NHibernateQueryableProvider(_session),
				JoinPathRegistry = new BaseJoinPathRegistry
				{
					MultipleJoinLookup = new Dictionary<Type, bool>()
				}
			};
		}

		[TearDown]
		public void TestTearDown()
		{
			_joinConfiguration = null;

			_session.Close();

			_sqlLiteBuilder.Dispose();
		}

		[Test]
		public void ToSqlStatement_NoJoinsNoWhere_ReturnsSqlStatement()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Sql<RootEntity>();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Parameters.Count, Is.EqualTo(0));
		}

		[Test]
		public void ToSqlStatement_NoJoinsWithWhere_ReturnsSqlStatement()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Where<RootEntity>(x => x.RootEntityField != "Hello World.")
				.Sql<RootEntity>();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Parameters.Count, Is.EqualTo(1));
		}

		[Test]
		public void ToSqlStatement_WithJoinsNoWhere_ReturnsSqlStatement()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Join<RootEntity>().To<OneLevelEntity>().On(p => p.RootEntityId, c => c.RootEntityParent.RootEntityId)
				.Sql<RootEntity>();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Parameters.Count, Is.EqualTo(0));
		}

		[Test]
		public void ToSqlStatement_WithJoinsWithWhere_ReturnsSqlStatement()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Join<RootEntity>().To<OneLevelEntity>().On(p => p.RootEntityId, c => c.RootEntityParent.RootEntityId)
				.Where<RootEntity>(x => x.RootEntityField != "Hello World.")
				.Sql<RootEntity>();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Parameters.Count, Is.EqualTo(1));
		}

		[Test]
		public void ToSqlStatement_WithJoinsWithWhereIsInRestriction_ReturnsSqlStatement()
		{
			var array = new[] {"Hello World.", "Goodbye World.", "Zomg World."};

			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Join<RootEntity>().To<OneLevelEntity>().On(p => p.RootEntityId, c => c.RootEntityParent.RootEntityId)
				.Where<RootEntity>(x => array.Contains(x.RootEntityField))
				.Sql<RootEntity>();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Parameters.Count, Is.EqualTo(3));
		}

		[Test]
		public void ToSqlStatement_WithJoinsWithWhereTwoParameters_ReturnsSqlStatement()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Join<RootEntity>().To<OneLevelEntity>().On(p => p.RootEntityId, c => c.RootEntityParent.RootEntityId)
				.Where<RootEntity>(x => x.RootEntityIntegerField < 13)
				.Where<RootEntity>(x => x.RootEntityField != "What?")
				.Sql<RootEntity>();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Parameters.Count, Is.EqualTo(2));
		}

		[Test]
		public void ToSqlStatement_StarSelect_ReturnsStarSelectSqlStatement()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Sql<RootEntity>(config => config.StarSelect());

			Assert.That(result, Is.Not.Null);
			Assert.That(result.SqlString.Contains("*"), Is.EqualTo(true));
			Assert.That(result.Parameters.Count, Is.EqualTo(0));
		}

		[Test]
		public void ToSqlStatement_Unalias_ReturnsUnaliasedSqlStatement()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Sql<RootEntity>(config => config.Unalias());

			Assert.That(result, Is.Not.Null);
			Assert.That(result.SqlString, Contains.Substring("ROOTENTITY_ROOTENTITYFIELD").IgnoreCase);
			Assert.That(result.SqlString, Contains.Substring("ROOTENTITY_ROOTENTITYINTEGERFIELD").IgnoreCase);
			Assert.That(result.SqlString, Contains.Substring("ROOTENTITY_ROOTENTITYBOOLEANFIELD").IgnoreCase);
			Assert.That(result.Parameters.Count, Is.EqualTo(0));
		}

		[Test]
		public void ToSqlStatement_Distinct_ReturnsDistinctSqlStatement()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Sql<RootEntity>(config => config.MakeDistinct());

			Assert.That(result, Is.Not.Null);
			Assert.That(result.SqlString.Contains("distinct"), Is.EqualTo(true));
			Assert.That(result.Parameters.Count, Is.EqualTo(0));
		}

		[Test]
		public void ToSqlStatement_Count_ReturnsCountSqlStatement()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Sql<RootEntity>(config => config.GetCount());

			Assert.That(result, Is.Not.Null);
			Assert.That(result.SqlString.Contains("count"), Is.EqualTo(true));
			Assert.That(result.Parameters.Count, Is.EqualTo(0));
		}

		[Test]
		public void ToSqlStatement_CountDistinct_ReturnsCountSqlStatement()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Sql<RootEntity>(config => config.CountDistinctProperty((RootEntity x) => x.RootEntityIntegerField));

			Assert.That(result, Is.Not.Null);
			Assert.That(result.SqlString.Contains("cast(count(distinct"), Is.EqualTo(true));
			Assert.That(result.SqlString.Contains("RootEntityIntegerField) as INT)"), Is.EqualTo(true));
			Assert.That(result.Parameters.Count, Is.EqualTo(0));
		}
	}
}
