﻿// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;

using Criteria.Expressions;
using Criteria.Joins;
using Criteria.Json;
using Criteria.NHibernate.IntegrationTests.App_Start;
using Criteria.NHibernate.IntegrationTests.Mocks;
using Criteria.NHibernate.IntegrationTests.SqlLite;
using Criteria.NHibernate.IntegrationTests.TreeModel;
using Criteria.Registries;

using NHibernate;

using NUnit.Framework;

using StructureMap;

namespace Criteria.NHibernate.IntegrationTests
{
	public class JoinTests
	{
		private ISession _session;
		private SqlLiteBuilder _sqlLiteBuilder;
		private JoinConfiguration _joinConfiguration;

		private void PerformSessionAction(Action<ISession> sessionAction)
		{
			using(var transaction = _session.BeginTransaction())
			{
				try
				{
					sessionAction(_session);
					transaction.Commit();
				}
				catch(Exception)
				{
					if(transaction.WasRolledBack == false)
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
				QueryableProvider = new NHibernateQueryableProvider(_session),
				ExpressionBuilder = new ExpressionBuilder(new TreeModelCriteriaTypeRegistry()),
				JoinPathRegistry = new EmptyJoinPathRegistry()
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
		public void Join_NoJoinsBuildsCorrectly()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Query<RootEntity>()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(1));
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
						Key = "RootEntityField",
						Operator = Operator.NotEqual,
						Value = "TestFieldValue"
					}
				}
			};

			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Where(criteria)
				.Query<RootEntity>()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Join_BuildsJoinExpressionCorrectly()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Join<RootEntity>().To<OneLevelEntity>().On(p => p.RootEntityId, c => c.RootEntityParent.RootEntityId)
				.Query<RootEntity>()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(2));
		}

		[Test]
		public void Join_JoinToTheSameType_ThrowsException()
		{
			Assert.That(() =>
					Join.Using(_joinConfiguration)
						.StartWith<RootEntity>()
						.Join<RootEntity>().To<RootEntity>().On(p => p.RootEntityId, c => c.RootEntityId)
						.Query<RootEntity>()
						.ToList()
				, Throws.ArgumentException);
		}

		[Test]
		public void Join_BuildsJoinExpressionWithJoinToNonRootEntitiesCorrectly()
		{
			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Join<RootEntity>().To<OneLevelEntity>().On(p => p.RootEntityId, c => c.RootEntityParent.RootEntityId)
				.Join<OneLevelEntity>().To<TwoLevelEntity>().On(o => o.RootEntityParent.RootEntityId, t => t.RootEntityParent.RootEntityId)
				.Query<RootEntity>()
				.Distinct()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Join_BuildsJoinExpressionWithWhereCorrectly()
		{
			var criteria = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Key = "RootEntityField",
						Operator = Operator.NotEqual,
						Value = "TestFieldValue"
					}
				}
			};

			var result = Join.Using(_joinConfiguration)
				.StartWith<RootEntity>()
				.Join<RootEntity>().To<OneLevelEntity>().On(p => p.RootEntityId, c => c.RootEntityParent.RootEntityId)
				.Join<RootEntity>().To<TwoLevelEntity>().On(p => p.RootEntityId, c => c.RootEntityParent.RootEntityId)
				.Where(criteria)
				.Query<RootEntity>()
				.Distinct()
				.ToList();

			Assert.That(result.Count(), Is.EqualTo(1));
		}
	}
}
