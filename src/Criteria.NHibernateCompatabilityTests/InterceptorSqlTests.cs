using System;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

using Criteria.Expressions;
using Criteria.Joins;
using Criteria.NHibernate;
using Criteria.NHibernateCompatabilityTests.Mocks;
using Criteria.NHibernateCompatabilityTests.SqlLite;
using Criteria.NHibernateCompatabilityTests.TreeModel;
using Criteria.NHibernateCompatabilityTests.TreeModel.Registries;
using Criteria.Sql;

using NHibernate;
using NHibernate.SqlCommand;

using NUnit.Framework;

using StructureMap;

namespace Criteria.NHibernateCompatabilityTests
{
	public class SqlTestException : Exception
	{
		public SqlTestException()
		{
		}

		public SqlTestException(string message) : base(message)
		{
		}

		public SqlTestException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	public class SqlEqualException : SqlTestException
	{
		public SqlEqualException()
		{
		}

		public SqlEqualException(string message) : base(message)
		{
		}

		public SqlEqualException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	public class SqlNotEqualException : SqlTestException
	{
		public SqlNotEqualException()
		{
		}

		public SqlNotEqualException(string message)
			: base(message)
		{
		}

		public SqlNotEqualException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	public class ExpectationInterceptor : EmptyInterceptor
	{
		public SqlGeneratorResult ExpectedSql { get; set; }

		private static bool AreNHibernateAliasedSqlStringsEqual(string actual, string expected)
		{
			var unAliasedActual = RemoveAliases(actual);
			var unAliasedExpected = RemoveAliases(expected);
			return unAliasedActual == unAliasedExpected;
		}

		private static string RemoveAliases(string sqlString)
		{
			return Regex.Replace(sqlString, @"_\d+_", @"__", RegexOptions.IgnoreCase);
		}

		public override SqlString OnPrepareStatement(SqlString sql)
		{
			if (AreNHibernateAliasedSqlStringsEqual(sql.ToString(), ExpectedSql.SqlString))
			{
				throw new SqlEqualException();
			}
			
			throw new SqlNotEqualException();
		}
	}

	[TestFixture]
	public class InterceptorSqlTests
	{
		private readonly ExpectationInterceptor _interceptor = new ExpectationInterceptor();

		private void PerformSessionAction(Action<ISession> sessionAction)
		{
			using (var sqlLiteBuilder = new SqlLiteBuilder(_interceptor))
			{
				using (var session = sqlLiteBuilder.GetSession())
				{
					using(var transaction = session.BeginTransaction())
					{
						try
						{
							sessionAction(session);

							transaction.Commit();
						}
						catch(Exception)
						{
							if((transaction.WasCommitted == false) && (transaction.WasRolledBack == false))
							{
								transaction.Rollback();
							}

							throw;
						}
					}

					session.Close();
				}

				sqlLiteBuilder.Dispose();
			}
		}

		private void PerformJoinAction(Action<Join> joinAction)
		{
			PerformSessionAction(session =>
			{
				var configuration = new JoinConfiguration
				{
					ExpressionBuilder = new ExpressionBuilder(new TreeModelCriteriaTypeRegistry()),
					QueryableProvider = new NHibernateQueryableProvider(session)
				};

				joinAction(Join.Using(configuration));
			});
		}

		private static void DigSqlTestExceptionOut(Action action)
		{
			try
			{
				action();
			}
			catch (ADOException adox)
			{
				Exception ex = adox.InnerException;
				while (!(ex is SqlTestException))
				{
					ex = ex.InnerException;
				}

				throw ex;
			}
		}

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			StructureMapBootstrapper.Bootstrap();

			if (ConfigurationManager.AppSettings["EnableNHibernateProfiler"].ToLower() == bool.TrueString.ToLower())
			{
				HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
			}

			ObjectFactory.Inject<IMappingAssemblyContainer>(new NHibernateMappingAssemblyContainer());
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			if (ConfigurationManager.AppSettings["EnableNHibernateProfiler"].ToLower() == bool.TrueString.ToLower())
			{
				HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Stop();
			}
		}

		[Test]
		public void NoJoins_GeneratesSameSqlAsNHibernate()
		{
			PerformJoinAction(join =>
			{
				_interceptor.ExpectedSql = join
					.StartWith<RootEntity>()
					.Sql<RootEntity>();
			});

			Assert.That(() =>
			{
				DigSqlTestExceptionOut(() =>
				{
					PerformJoinAction(join =>
					{
						join
							.StartWith<RootEntity>()
							.Query<RootEntity>()
							.ToList();
					});
				});
			}, Throws.InstanceOf<SqlEqualException>());
		}

		[Test]
		public void SingleJoin_GeneratesSameSqlAsNHibernate()
		{
			PerformJoinAction(join =>
			{
				_interceptor.ExpectedSql = join
					.StartWith<RootEntity>()
					.Join<RootEntity>().To<OneLevelEntity>().On(root => root.RootEntityId, one => one.RootEntityParent.RootEntityId)
					.Sql<RootEntity>();
			});

			Assert.That(() =>
			{
				DigSqlTestExceptionOut(() =>
				{
					PerformJoinAction(join =>
					{
						join
							.StartWith<RootEntity>()
							.Join<RootEntity>().To<OneLevelEntity>().On(root => root.RootEntityId, one => one.RootEntityParent.RootEntityId)
							.Query<RootEntity>()
							.ToList();
					});
				});
			}, Throws.InstanceOf<SqlEqualException>());
		}

		[Test]
		public void MultipleJoins_GeneratesSameSqlAsNHibernate()
		{
			PerformJoinAction(join =>
			{
				_interceptor.ExpectedSql = join
					.StartWith<RootEntity>()
					.Join<RootEntity>().To<OneLevelEntity>().On(root => root.RootEntityId, one => one.RootEntityParent.RootEntityId)
					.Join<RootEntity>().To<TwoLevelEntity>().On(root => root.RootEntityId, two => two.RootEntityParent.RootEntityId)
					.Join<TwoLevelEntity>().To<TwoLevelEntityChild>().On(two => two.TwoLevelEntityId, child => child.TwoLevelEntityParent.TwoLevelEntityId)
					.Sql<RootEntity>();
			});

			Assert.That(() =>
			{
				DigSqlTestExceptionOut(() =>
				{
					PerformJoinAction(join =>
					{
						join
							.StartWith<RootEntity>()
							.Join<RootEntity>().To<OneLevelEntity>().On(root => root.RootEntityId, one => one.RootEntityParent.RootEntityId)
							.Join<RootEntity>().To<TwoLevelEntity>().On(root => root.RootEntityId, two => two.RootEntityParent.RootEntityId)
							.Join<TwoLevelEntity>().To<TwoLevelEntityChild>().On(two => two.TwoLevelEntityId, child => child.TwoLevelEntityParent.TwoLevelEntityId)
							.Query<RootEntity>()
							.ToList();
					});
				});
			}, Throws.InstanceOf<SqlEqualException>());
		}
	}
}
