// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Criteria.Expressions;
using Criteria.Expressions.Impl;
using Criteria.Json;
using Criteria.NHibernate.IntegrationTests.App_Start;
using Criteria.NHibernate.IntegrationTests.SqlLite;
using Criteria.NHibernate.IntegrationTests.TreeModel;

using Newtonsoft.Json;

using NHibernate;
using NHibernate.Linq;

using NUnit.Framework;

namespace Criteria.NHibernate.IntegrationTests
{
	[TestFixture]
	public class CriteriaLeafExpressionStrategyIntegrationTests
	{
		private ISession _session;
		private SqlLiteBuilder _sqlLiteBuilder;
		private ICriteriaLeafExpressionStrategy _expressionStrategy;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			StructureMapBootstrapper.Bootstrap();

			_expressionStrategy = new CriteriaLeafExpressionStrategy(new TreeModelCriteriaTypeRegistry());

			NHibernateProfilerBootstrapper.PreStart();
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
		}

		[TearDown]
		public void TestTearDown()
		{
			_sqlLiteBuilder.Dispose();
		}

		public void PerformSessionAction(Action<ISession> sessionAction)
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
				}
			}
		}

		private void InsertTestData()
		{
			PerformSessionAction(session =>
			{
				session.Save(new RootEntity { RootEntityField = "TestFieldValue", RootEntityIntegerField = 0, RootEntityBooleanField = true });
				session.Save(new RootEntity { RootEntityField = "FieldValueTwo", RootEntityIntegerField = 1 });
				session.Save(new RootEntity
				{
					RootEntityField = "RootEntityFieldValue",
					RootEntityIntegerField = 2,
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

		[Test]
		public void GetExpression_CriteriaLeafWithEqualsOperator_EqualsExpression()
		{
			var testPropertyName = "RootEntityField";
			var testOperator = (int)Operator.Equal;
			var testValue = "TestFieldValue";
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithNotEqualsOperator_NotEqualsExpression()
		{
			var testPropertyName = "RootEntityField";
			var testOperator = (int)Operator.NotEqual;
			var testValue = "TestFieldValue";
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithLessThanOperator_LessThanExpression()
		{
			var testPropertyName = "RootEntityIntegerField";
			var testOperator = (int)Operator.LessThan;
			var testValue = 2;
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithLessThanOrEqualToOperator_LessThanOrEqualExpression()
		{
			var testPropertyName = "RootEntityIntegerField";
			var testOperator = (int)Operator.LessThanOrEqual;
			var testValue = 1;
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithGreaterThanOperator_GreaterThanExpression()
		{
			var testPropertyName = "RootEntityIntegerField";
			var testOperator = (int)Operator.GreaterThan;
			var testValue = 1;
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithGreaterThanOrEqualToOperator_GreaterThanOrEqualExpression()
		{
			var testPropertyName = "RootEntityIntegerField";
			var testOperator = (int)Operator.GreaterThanOrEqual;
			var testValue = 1;
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsInOperator_NewArrayContainsExpression()
		{
			var testPropertyName = "RootEntityField";
			var testOperator = (int)Operator.IsIn;
			var valueArray = new[] { "'TestFieldValue'", "'NotInDatabaseValue'" };
			var testValue = String.Format("[{0}]", String.Join(",", valueArray));
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsNotInOperator_NewArrayContainsExpression()
		{
			var testPropertyName = "RootEntityField";
			var testOperator = (int)Operator.IsNotIn;
			var valueArray = new[] { "'TestFieldValue'", "'NotInDatabaseValue'" };
			var testValue = String.Format("[{0}]", String.Join(",", valueArray));
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithContainsOperator_ContainsExpression()
		{
			var testPropertyName = "RootEntityField";
			var testOperator = (int)Operator.Contains;
			var testValue = "Entity";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithDoesNotContainOperator_ContainsExpression()
		{
			var testPropertyName = "RootEntityField";
			var testOperator = (int)Operator.DoesNotContain;
			var testValue = "Test";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithStartsWithOperator_StartsWithExpression()
		{
			var testPropertyName = "RootEntityField";
			var testOperator = (int)Operator.StartsWith;
			var testValue = "Test";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithDoesNotStartWithOperator_DoesNotStartWithExpression()
		{
			var testPropertyName = "RootEntityField";
			var testOperator = (int)Operator.DoesNotStartWith;
			var testValue = "Test";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithEndsWithOperator_EndsWithExpression()
		{
			var testPropertyName = "RootEntityField";
			var testOperator = (int)Operator.EndsWith;
			var testValue = "Value";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithDoesNotEndWithOperator_EndsWithExpression()
		{
			var testPropertyName = "RootEntityField";
			var testOperator = (int)Operator.DoesNotEndWith;
			var testValue = "Value";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithBetweenOperator_BetweenExpression()
		{
			var testPropertyName = "RootEntityIntegerField";
			var testOperator = (int)Operator.Between;
			var testValue = String.Format("[{0}]", String.Join(",", new []{1, 1}));
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsTrueOperator_IsTrueExpression()
		{
			var testPropertyName = "RootEntityBooleanField";
			var testOperator = (int)Operator.IsTrue;

			var jsonString = String.Format("{{key: '{0}', operator: {1}}}", testPropertyName, testOperator);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(1));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsFalseOperator_IsFalseExpression()
		{
			var testPropertyName = "RootEntityBooleanField";
			var testOperator = (int)Operator.IsFalse;

			var jsonString = String.Format("{{key: '{0}', operator: {1}}}", testPropertyName, testOperator);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var body = _expressionStrategy.GetExpression(criteriaLeaf);

			var expr = (Expression<Func<RootEntity, bool>>)Expression.Lambda(body, false, new[] { Expression.Parameter(typeof(RootEntity), "x") });

			IEnumerable<RootEntity> result = null;

			PerformSessionAction(session =>
			{
				result = session.Query<RootEntity>()
					.Where(expr)
					.ToList();
			});

			Assert.That(result.Count(), Is.EqualTo(2));
		}
	}
}
