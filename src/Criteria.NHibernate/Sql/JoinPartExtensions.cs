using System;

using Criteria.Joins;
using Criteria.Sql;

using StructureMap;

namespace Criteria.NHibernate.Sql
{
	public static class JoinPartExtensions
	{
		public static SqlGeneratorResult Sql<TResult>(this AbstractJoinPart joinPart)
		{
			var sqlGenerator = ObjectFactory.GetInstance<ISqlGenerator>();

			return joinPart.Sql<TResult>(sqlGenerator, new SqlGeneratorConfiguration());
		}

		public static SqlGeneratorResult Sql<TResult>(this AbstractJoinPart joinPart, Action<SqlGeneratorConfigurator> configure)
		{
			var sqlGenerator = ObjectFactory.GetInstance<ISqlGenerator>();

			var configuration = new SqlGeneratorConfiguration();

			var configurator = new SqlGeneratorConfigurator(configuration);

			configure(configurator);

			return joinPart.Sql<TResult>(sqlGenerator, configuration);
		}
	}
}
