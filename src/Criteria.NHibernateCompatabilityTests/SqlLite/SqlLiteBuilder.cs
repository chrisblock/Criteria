using System;
using System.Configuration;
using System.IO;

using FluentNHibernate.Cfg;

using NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;

using Configuration = NHibernate.Cfg.Configuration;
using Environment = NHibernate.Cfg.Environment;

namespace Criteria.NHibernateCompatabilityTests.SqlLite
{
	public class SqlLiteBuilder : IDisposable
	{
		private static ISessionFactory _sessionFactory;
		private readonly ISession _session;

		public SqlLiteBuilder()
		{
			var configuration = BuildConfiguration();

			_sessionFactory = Fluently.Configure(configuration)
				.Mappings(mappings => mappings.FluentMappings.AddFromAssemblyOf<SqlLiteBuilder>())
				.BuildSessionFactory();

			_session = _sessionFactory.OpenSession();

			TextWriter output = null;

			if (ConfigurationManager.AppSettings["ShowSqlLiteSchemaStatements"].ToLower() == bool.TrueString.ToLower())
			{
				output = Console.Out;
			}

			new SchemaExport(configuration).Execute(false, true, false, _session.Connection, output);
		}

		public SqlLiteBuilder(IInterceptor interceptor)
		{
			var configuration = BuildConfiguration(interceptor);

			_sessionFactory = Fluently.Configure(configuration)
				.Mappings(mappings => mappings.FluentMappings.AddFromAssemblyOf<SqlLiteBuilder>())
				.BuildSessionFactory();

			_session = _sessionFactory.OpenSession();

			TextWriter output = null;

			if (ConfigurationManager.AppSettings["ShowSqlLiteSchemaStatements"].ToLower() == bool.TrueString.ToLower())
			{
				output = Console.Out;
			}

			new SchemaExport(configuration).Execute(false, true, false, _session.Connection, output);
		}

		private static Configuration BuildConfiguration()
		{
			var configuration = new Configuration()
				.Proxy(proxy => proxy.ProxyFactoryFactory<ProxyFactoryFactory>())
				.DataBaseIntegration(db =>
				{
					db.Dialect<SQLiteDialect>();
					db.Driver<SQLite20Driver>();
					db.ConnectionString = "data source=:memory:";
				})
				.SetProperty(Environment.ReleaseConnections, "on_close")
				.SetProperty(Environment.ShowSql, ConfigurationManager.AppSettings["ShowSqlQueryStatements"].ToLower());

			return configuration;
		}

		private static Configuration BuildConfiguration(IInterceptor interceptor)
		{
			var configuration = BuildConfiguration();

			configuration.SetInterceptor(interceptor);

			return configuration;
		}

		public ISession GetSession()
		{
			return _session;
		}

		public void Dispose()
		{
			_session.Dispose();

			_sessionFactory.Dispose();
		}
	}
}
