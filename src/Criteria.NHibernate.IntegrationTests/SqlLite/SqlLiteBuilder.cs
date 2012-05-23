using System;
using System.Configuration;
using System.IO;
using System.Reflection;

using FluentNHibernate.Cfg;

using NHibernate;
using NHibernate.Bytecode;
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
			var showSql = GetShowSql();

			var configuration = new Configuration()
				.Proxy(p => p.ProxyFactoryFactory<DefaultProxyFactoryFactory>())
				.DataBaseIntegration(db =>
				{
					db.Dialect<SQLiteDialect>();
					db.Driver<SQLite20Driver>();
					db.ConnectionString = "data source=:memory:";
				})
				.SetProperty(Environment.ReleaseConnections, "on_close")
				.SetProperty(Environment.ShowSql, showSql)
				.AddAssembly(Assembly.GetCallingAssembly());

			_sessionFactory = Fluently.Configure(configuration)
				.Mappings(mappings => mappings.FluentMappings.AddFromAssemblyOf<SqlLiteBuilder>())
				.BuildSessionFactory();

			_session = _sessionFactory.OpenSession();

			var textWriter = GetTextWriter();

			var schemaExport = new SchemaExport(configuration);
			
			schemaExport.Execute(false, true, false, _session.Connection, textWriter);
		}

		private static string GetShowSql()
		{
			var profile = ConfigurationManager.AppSettings["ProfileNHibernate"];

			var result = "false";

			if(String.IsNullOrWhiteSpace(profile) == false)
			{
				bool show;
				if(Boolean.TryParse(profile, out show) && show)
				{
					result = "true";
				}
			}

			return result;
		}

		private static TextWriter GetTextWriter()
		{
			TextWriter result = null;

			var config = ConfigurationManager.AppSettings["ProfileNHibernate"];

			if(String.IsNullOrWhiteSpace(config) == false)
			{
				bool profile;
				if(Boolean.TryParse(config, out profile) && profile)
				{
					result = Console.Out;
				}
			}

			return result;
		}

		public ISession GetSession()
		{
			return _session;
		}

		public void Dispose()
		{
			_session.Dispose();
		}
	}
}
