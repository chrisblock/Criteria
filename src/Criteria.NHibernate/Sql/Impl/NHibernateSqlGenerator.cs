using Criteria.Sql;

using FluentNHibernate.Cfg;

using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Engine;

namespace Criteria.NHibernate.Sql.Impl
{
	public class NHibernateSqlGenerator<TDialect, TDriver> : BaseNHibernateSqlGenerator
		where TDialect : Dialect
		where TDriver : IDriver
	{
		private readonly ISessionFactoryImplementor _sessionFactoryImplementor;

		protected override ISessionFactoryImplementor SessionFactory
		{
			get { return _sessionFactoryImplementor; }
		}

		public NHibernateSqlGenerator(IMappingAssemblyContainer mappingAssemblies, string connectionString)
		{
			var configuration = new Configuration()
				.Proxy(p => p.ProxyFactoryFactory<DefaultProxyFactoryFactory>())
				.DataBaseIntegration(db =>
				{
					db.Dialect<TDialect>();
					db.Driver<TDriver>();
					db.ConnectionString = connectionString;
				});

			_sessionFactoryImplementor = (ISessionFactoryImplementor)Fluently.Configure(configuration)
				.Mappings(mappings =>
				{
					foreach (var mappingAssembly in mappingAssemblies.GetMappingAssemblies())
					{
						mappings.FluentMappings.AddFromAssembly(mappingAssembly);
					}
				})
				.BuildSessionFactory();
		}
	}
}
