using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

using Criteria.Sql;

using NHibernate;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Impl;
using NHibernate.Linq;
using NHibernate.Loader;
using NHibernate.Loader.Criteria;
using NHibernate.Persister.Entity;

using IQueryable = NHibernate.Persister.Entity.IQueryable;

namespace Criteria.NHibernate.Sql.Impl
{
	public abstract class BaseNHibernateSqlGenerator : ISqlGenerator
	{
		private const BindingFlags InstanceNonPublicBinding = BindingFlags.Instance | BindingFlags.NonPublic;

		protected abstract ISessionFactoryImplementor SessionFactory { get; }

		public virtual SqlGeneratorResult Generate(Expression expression)
		{
			return GenerateSql(expression, false, false);
		}

		public virtual SqlGeneratorResult GenerateUnaliased(Expression expression)
		{
			return GenerateSql(expression, true, false);
		}

		public virtual SqlGeneratorResult GenerateStarSelect(Expression expression)
		{
			return GenerateSql(expression, false, true);
		}

		private SqlGeneratorResult GenerateSql(Expression expression, bool unalias, bool isStarSelect)
		{
			SqlGeneratorResult result;

			// TODO: Try to reuse an existing session, if one exists
			using (var session = SessionFactory.OpenSession())
			{
				var sessionImplementor = session as ISessionImplementor;

				if (sessionImplementor == null)
				{
					throw new InvalidCastException("Sql generation only works with ISessionImplementor sessions.");
				}

				var queryExpressionAndParameters = GetQueryExpressionWithParameters(sessionImplementor, expression);

				var queryExpression = queryExpressionAndParameters.Item1;
				var queryParameters = queryExpressionAndParameters.Item2;

				var dbCommand = CreateDbCommand(sessionImplementor, queryExpression, queryParameters, unalias, isStarSelect);

				result = new SqlGeneratorResult(dbCommand);

				session.Close();
			}

			return result;
		}

		private static System.Tuple<IQueryExpression, QueryParameters> GetQueryExpressionWithParameters(ISessionImplementor sessionImplementor, Expression expression)
		{
			var query = GetQueryFromExpression(sessionImplementor, expression);

			var namedParamsCopy = (IDictionary<string, TypedValue>)query.GetType().GetProperty("NamedParams", InstanceNonPublicBinding).GetGetMethod(true).Invoke(query, new object[0]);

			var expandParametersParameters = new object[] { namedParamsCopy };

			var queryExpression = query.GetType().GetMethod("ExpandParameters", InstanceNonPublicBinding).Invoke(query, expandParametersParameters) as IQueryExpression;

			namedParamsCopy = expandParametersParameters[0] as IDictionary<string, TypedValue>;

			var abstractQuery = query as AbstractQueryImpl;

			if(abstractQuery == null)
			{
				throw new InvalidCastException("Sql generation only works with AbstractQueryImpl queries.");
			}

			var queryParameters = abstractQuery.GetQueryParameters(namedParamsCopy);

			return new System.Tuple<IQueryExpression, QueryParameters>(queryExpression, queryParameters);
		}

		private static IQuery GetQueryFromExpression(ISessionImplementor sessionImplementor, Expression expression)
		{
			var provider = new DefaultQueryProvider(sessionImplementor);
			IQuery query = null;
			NhLinqExpression nhQuery = null;

			var parameters = new object[] {expression, query, nhQuery};
			var parameterTypes = new[]
			{
				typeof(Expression),
				typeof(IQuery).MakeByRefType(),
				typeof(NhLinqExpression).MakeByRefType()
			};

			provider.GetType()
				.GetMethod("PrepareQuery", InstanceNonPublicBinding, null, parameterTypes, null)
				.Invoke(provider, parameters);
			
			query = parameters[1] as IQuery;

			return query;
		}

		private IDbCommand CreateDbCommand(ISessionImplementor sessionImplementor, IQueryExpression queryExpression, QueryParameters queryParameters, bool unalias, bool isStarSelect)
		{
			var enabledFilters = new Dictionary<string, IFilter>();
			var queryPlan = SessionFactory.QueryPlanCache.GetHQLQueryPlan(queryExpression, false, enabledFilters);

			var queryTranslator = queryPlan.Translators.Single();

			queryTranslator.Compile(SessionFactory.Settings.QuerySubstitutions, false);

			var loader = queryTranslator.Loader;

			var result = (IDbCommand)loader.GetType().GetMethod("PrepareQueryCommand", InstanceNonPublicBinding).Invoke(loader, new object[] { queryParameters, false, sessionImplementor });

			// TODO: This is a little ridiculous...
			if(unalias)
			{
				var columnNamesAndAliases = GetColumnNamesAndColumnAliases(queryTranslator);

				var unaliasedSqlString = UnaliasSqlString(result.CommandText, columnNamesAndAliases);

				result.CommandText = unaliasedSqlString;
			}
			else if (isStarSelect)
			{
				var aliases = (string[])loader.GetType().GetProperty("Aliases", InstanceNonPublicBinding | BindingFlags.GetProperty).GetValue(loader, new object[0]);
				var tableAlias = aliases.FirstOrDefault();
				var starSelectSqlString = GenerateStarSelectSqlString(result.CommandText, tableAlias);

				result.CommandText = starSelectSqlString;
			}

			return result;
		}

		private static string GenerateStarSelectSqlString(string sqlString, string tableAlias)
		{
			var indexOfFrom = sqlString.IndexOf(" from ", System.StringComparison.Ordinal);

			var starSelectSqlString = String.Format("select {0}.*{1}", tableAlias, sqlString.Substring(indexOfFrom));

			return starSelectSqlString;
		}

		private static IDictionary<string, string> GetColumnNamesAndColumnAliases(IQueryTranslator translator)
		{
			var loader = translator.Loader;

			var entityPersisters = loader.EntityPersisters;

			var entityAliases = (IEntityAliases[])loader.GetType().GetProperty("EntityAliases", InstanceNonPublicBinding | BindingFlags.GetProperty).GetValue(loader, new object[0]);

			var columnNames = entityPersisters[0].PropertyNames.Select(x => x).ToList();

			var mappedColumns = GetMappedColumns(entityPersisters[0], columnNames);
			mappedColumns.AddRange(entityPersisters[0].IdentifierColumnNames);

			var columnAliases = entityAliases[0].SuffixedPropertyAliases.Select(x =>
			{
				return x.Length > 0 ? x[0] : String.Empty;
			}).ToList();

			columnAliases.AddRange(entityAliases[0].SuffixedKeyAliases);

			if(mappedColumns.Count != columnAliases.Count)
			{
				throw new ApplicationException("Cannot unalias sql when column alias count are not equal to mapped column count");
			}

			var result = new Dictionary<string, string>();
			for(var i = 0; i < columnAliases.Count; i++)
			{
				var badAlias = columnAliases[i];
				var newGoodAlias = mappedColumns[i];

				if(String.IsNullOrWhiteSpace(badAlias) || String.IsNullOrWhiteSpace(newGoodAlias))
				{
					continue;
				}
				
				if (result.ContainsKey(badAlias))
				{
					throw new ApplicationException(String.Format("Cannot unalias sql because of duplicate alias: {0}", badAlias));
				}

				result[badAlias] = newGoodAlias;
			}

			return result;
		}

		private static List<string> GetMappedColumns(IEntityPersister persister, IEnumerable<string> columnNames)
		{
			var criteriaInfoProvider = new EntityCriteriaInfoProvider((IQueryable)persister);

			var propertySpaces = persister.PropertySpaces;
			var propertyTableNumbersInSelect = (int[])persister.GetType().GetProperty("PropertyTableNumbersInSelect", InstanceNonPublicBinding | BindingFlags.GetProperty).GetValue(persister, new object[0]);

			var result = columnNames.Select((x, index) =>
			{
				return String.IsNullOrWhiteSpace(x) ? String.Empty : GenerateAlias(criteriaInfoProvider.PropertyMapping.ToColumns(x)[0], propertySpaces, propertyTableNumbersInSelect[index]);
			}).ToList();

			return result;
		}

		private static string GenerateAlias(string mappedColumnName, IList<string> propertySpaces, int targetPropertySpaceIndex)
		{
			var result = String.Empty;

			if(String.IsNullOrWhiteSpace(mappedColumnName) == false)
			{
				var tableName = propertySpaces[targetPropertySpaceIndex];

				result = String.Format("{0}_{1}", tableName, mappedColumnName);
			}

			return result.ToUpper();
		}

		private static string UnaliasSqlString(string sqlString, IDictionary<string, string> columnNamesAndAliases)
		{
			var badColumnAliases = columnNamesAndAliases.Keys;

			var aliasExpression = String.Format("({0})", String.Join("|", badColumnAliases.Where(x => String.IsNullOrWhiteSpace(x) == false)));

			var splitted = Regex.Split(sqlString, aliasExpression, RegexOptions.IgnoreCase);

			for (var i = 0; i < splitted.Length; i++ )
			{
				var badAlias = splitted[i];
				if(columnNamesAndAliases.ContainsKey(badAlias))
				{
					splitted[i] = columnNamesAndAliases[badAlias];
				}
			}

			var result = String.Join("", splitted);

			return result;
		}
	}
}
