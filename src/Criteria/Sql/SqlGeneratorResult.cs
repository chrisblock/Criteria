using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Criteria.Sql.Impl;

namespace Criteria.Sql
{
	public class SqlGeneratorResult
	{
		private const string NamedParameterExpression = @"@p\d+";

		public string SqlString { get; set; }

		public ICollection<IDataParameter> Parameters { get; set; }

		private readonly IDictionary<DbType, ISqlUnparameterizationStrategy> _strategyRegistry;

		public SqlGeneratorResult()
		{
			Parameters = new List<IDataParameter>();

			var quotedStrategy = new QuotedUnparameterizationStrategy();
			var nonQuotedStrategy = new NonQuotedUnparameterizationStrategy();
			var escapedStringStrategy = new EscapedStringUnparameterizationStrategy();

			_strategyRegistry = new Dictionary<DbType, ISqlUnparameterizationStrategy>
			{
				{ DbType.AnsiString, escapedStringStrategy },
				{ DbType.Byte, nonQuotedStrategy },
				{ DbType.Boolean, nonQuotedStrategy },
				{ DbType.Currency, nonQuotedStrategy },
				{ DbType.Date, quotedStrategy },
				{ DbType.DateTime, quotedStrategy },
				{ DbType.Decimal, nonQuotedStrategy },
				{ DbType.Double, nonQuotedStrategy },
				{ DbType.Guid, quotedStrategy },
				{ DbType.Int16, nonQuotedStrategy },
				{ DbType.Int32, nonQuotedStrategy },
				{ DbType.Int64, nonQuotedStrategy },
				{ DbType.SByte, nonQuotedStrategy },
				{ DbType.Single, nonQuotedStrategy },
				{ DbType.String, escapedStringStrategy },
				{ DbType.Time, quotedStrategy },
				{ DbType.UInt16, nonQuotedStrategy },
				{ DbType.UInt32, nonQuotedStrategy },
				{ DbType.UInt64, nonQuotedStrategy },
				{ DbType.VarNumeric, nonQuotedStrategy },
				{ DbType.AnsiStringFixedLength, escapedStringStrategy },
				{ DbType.StringFixedLength, escapedStringStrategy },
				{ DbType.DateTime2, quotedStrategy }
			};
		}

		public SqlGeneratorResult(IDbCommand dbCommand) : this()
		{
			SqlString = dbCommand.CommandText;
			Parameters = dbCommand.Parameters.Cast<IDataParameter>().ToList();
		}

		public string GetUnparameterizedSql()
		{
			if((Parameters == null) || (Parameters.Count == 0))
			{
				return SqlString;
			}

			var result = SqlString.Contains("?")
					? UnparameterizePositionalParameters()
					: UnparameterizeNamedParameters();

			return result;
		}

		private string UnparameterizePositionalParameters()
		{
			var sqlParts = SqlString.Split(new[] { '?' });

			if (sqlParts.Length != (Parameters.Count + 1))
			{
				throw new ArgumentException("Unable to determine parameter replacement strategy.");
			}

			var builder = new StringBuilder();
			builder.Append(sqlParts[0]);

			for (var i = 0; i < Parameters.Count; ++i)
			{
				var parameterStringValue = GetParameterStringValue(Parameters.ElementAt(i));
				builder.Append(parameterStringValue);
				builder.Append(sqlParts[i + 1]);
			}

			return builder.ToString();
		}

		private string UnparameterizeNamedParameters()
		{
			var matches = Regex.Matches(SqlString, NamedParameterExpression);

			var sqlParts = Regex.Split(SqlString, NamedParameterExpression, RegexOptions.IgnoreCase);

			if (matches.Count != Parameters.Count)
			{
				throw new ArgumentException("Unable to determine parameter replacement strategy.");
			}

			var builder = new StringBuilder();
			builder.Append(sqlParts[0]);

			for (var i = 0; i < matches.Count; ++i)
			{
				var match = matches[i];
				var parameterName = match.Value;
				var parameterValue = Parameters
										.Where(x => x.ParameterName.Replace("@", String.Empty) == parameterName.Replace("@", String.Empty))
										.Select(GetParameterStringValue)
										.Single();

				builder.Append(parameterValue);
				builder.Append(sqlParts[i + 1]);
			}

			return builder.ToString();
		}

		private string GetParameterStringValue(IDataParameter parameter)
		{
			var result = "NULL";

			if(parameter.Value != null)
			{
				var parameterType = parameter.DbType;
				if(_strategyRegistry.ContainsKey(parameterType))
				{
					result = _strategyRegistry[parameterType].Unparameterize(parameter);
				}
				else
				{
					throw new ArgumentException(String.Format("Unknown parameter replacement strategy for DbType: {0}", parameterType));
				}
			}

			return result;
		}
	}
}
