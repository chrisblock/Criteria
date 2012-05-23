using Criteria.NHibernate.Sql.Impl;
using Criteria.Sql;

using NHibernate.Dialect;
using NHibernate.Driver;

using StructureMap.Configuration.DSL;

namespace Criteria.NHibernate
{
	public class CriteriaNHibernateRegistry : Registry
	{
		public CriteriaNHibernateRegistry()
		{
			Scan(scan =>
			{
				scan.AssemblyContainingType<CriteriaNHibernateRegistry>();
				scan.WithDefaultConventions();
			});

			IncludeRegistry<CriteriaRegistry>();

			For<ISqlGenerator>()
				.Use<NHibernateSqlGenerator<SQLiteDialect, SQLite20Driver>>()
				.Ctor<string>("connectionString").Is("Data Source=:memory:;");
		}
	}
}
