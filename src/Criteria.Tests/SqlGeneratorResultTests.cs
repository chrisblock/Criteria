// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Criteria.Sql;

using NUnit.Framework;

namespace Criteria.Tests
{
	[TestFixture]
	public class SqlGeneratorResultTests
	{
		[Test]
		public void GetUnparameterizedSql_NoParameters_UnparameterizesCorrectly()
		{
			var sqlGeneratorResult = new SqlGeneratorResult
			{
				SqlString = "SELECT 10",
				Parameters = new List<IDataParameter>()
			};

			var unparameterizedSql = sqlGeneratorResult.GetUnparameterizedSql();

			Assert.That(unparameterizedSql, Is.EqualTo("SELECT 10"));
		}

		[Test]
		public void GetUnparameterizedSql_PositionalParameters_UnparameterizesCorrectly()
		{
			var sqlGeneratorResult = new SqlGeneratorResult
			{
				SqlString = "SELECT column1, column2 FROM table1 WHERE column3 < ? and column1 > ?",
				Parameters = new List<IDataParameter>
				{
					new SqlParameter("parameterName1", "value1"),
					new SqlParameter("parameterName2", 3)
				}
			};

			var unparameterizedSql = sqlGeneratorResult.GetUnparameterizedSql();

			Assert.That(unparameterizedSql, Is.EqualTo("SELECT column1, column2 FROM table1 WHERE column3 < 'value1' and column1 > 3"));
		}

		[Test]
		public void GetUnparameterizedSql_PositionalParametersWithNull_UnparameterizesCorrectly()
		{
			var sqlGeneratorResult = new SqlGeneratorResult
			{
				SqlString = "SELECT column1, column2 FROM table1 WHERE column3 < ? and column1 IS NOT ?",
				Parameters = new List<IDataParameter>
				{
					new SqlParameter("parameterName1", "value1"),
					new SqlParameter("parameterName2", null)
				}
			};

			var unparameterizedSql = sqlGeneratorResult.GetUnparameterizedSql();

			Assert.That(unparameterizedSql, Is.EqualTo("SELECT column1, column2 FROM table1 WHERE column3 < 'value1' and column1 IS NOT NULL"));
		}

		[Test]
		public void GetUnparameterizedSql_NotEnoughPositionalParameters_Throws()
		{
			var sqlGeneratorResult = new SqlGeneratorResult
			{
				SqlString = "SELECT column1, column2 FROM table1 WHERE column3 < ? and column1 in (?, ?)",
				Parameters = new List<IDataParameter>
				{
					new SqlParameter("parameterName1", "value1"),
					new SqlParameter("parameterName2", 3)
				}
			};

			Assert.Throws<ArgumentException>(() => sqlGeneratorResult.GetUnparameterizedSql());
		}

		[Test]
		public void GetUnparameterizedSql_TooManyPositionalParameters_Throws()
		{
			var sqlGeneratorResult = new SqlGeneratorResult
			{
				SqlString = "SELECT column1, column2 FROM table1 WHERE column3 < ? and column1 > ?",
				Parameters = new List<IDataParameter>
				{
					new SqlParameter("parameterName1", "value1"),
					new SqlParameter("parameterName2", 3),
					new SqlParameter("parameterName3", 4)
				}
			};

			Assert.Throws<ArgumentException>(() => sqlGeneratorResult.GetUnparameterizedSql());
		}

		[Test]
		public void GetUnparameterizedSql_UnknownParameterType_Throws()
		{
			var sqlGeneratorResult = new SqlGeneratorResult
			{
				SqlString = "SELECT column1, column2 FROM table1 WHERE column3 < ? and column1 > ?",
				Parameters = new List<IDataParameter>
				{
					new SqlParameter("parameterName1", "value1"),
					new SqlParameter("parameterName3", 3) { DbType = DbType.Object }
				}
			};

			Assert.Throws<ArgumentException>(() => sqlGeneratorResult.GetUnparameterizedSql());
		}

		[Test]
		public void GetUnparameterizedSql_NamedParameters_UnparameterizesCorrectly()
		{
			var sqlGeneratorResult = new SqlGeneratorResult
			{
				SqlString = "SELECT column1, column2 FROM table1 WHERE column3 < @p1 and column1 > @p11",
				Parameters = new List<IDataParameter>
				{
					new SqlParameter("p1", "value1"),
					new SqlParameter("p11", 3)
				}
			};

			var unparameterizedSql = sqlGeneratorResult.GetUnparameterizedSql();

			Assert.That(unparameterizedSql, Is.EqualTo("SELECT column1, column2 FROM table1 WHERE column3 < 'value1' and column1 > 3"));
		}

		[Test]
		public void GetUnparameterizedSql_NamedParametersWithAtSign_UnparameterizesCorrectly()
		{
			var sqlGeneratorResult = new SqlGeneratorResult
			{
				SqlString = "SELECT column1, column2 FROM table1 WHERE column3 < @p11 and column1 > @p1",
				Parameters = new List<IDataParameter>
				{
					new SqlParameter("@p1", "value1"),
					new SqlParameter("@p11", 3)
				}
			};

			var unparameterizedSql = sqlGeneratorResult.GetUnparameterizedSql();

			Assert.That(unparameterizedSql, Is.EqualTo("SELECT column1, column2 FROM table1 WHERE column3 < 3 and column1 > 'value1'"));
		}

		[Test]
		public void GetUnparameterizedSql_NotEnoughNamedParameters_Throws()
		{
			var sqlGeneratorResult = new SqlGeneratorResult
			{
				SqlString = "SELECT column1, column2 FROM table1 WHERE column3 < @p1 and column1 in (@p2, @p3)",
				Parameters = new List<IDataParameter>
				{
					new SqlParameter("p1", "value1"),
					new SqlParameter("p2", 3)
				}
			};

			Assert.Throws<ArgumentException>(() => sqlGeneratorResult.GetUnparameterizedSql());
		}

		[Test]
		public void GetUnparameterizedSql_TooManyNamedParameters_Throws()
		{
			var sqlGeneratorResult = new SqlGeneratorResult
			{
				SqlString = "SELECT column1, column2 FROM table1 WHERE column3 < @p1 and column1 > @p2",
				Parameters = new List<IDataParameter>
				{
					new SqlParameter("p1", "value1"),
					new SqlParameter("p2", 3),
					new SqlParameter("p3", 4)
				}
			};

			Assert.Throws<ArgumentException>(() => sqlGeneratorResult.GetUnparameterizedSql());
		}

		[Test]
		public void Constructor_DbCommand_UpdatesPropertyValues()
		{
			const string commandText = "CommandText";
			var parameter = new SqlParameter("parameterName1", "value1");

			var command = new SqlCommand(commandText);
			command.Parameters.Add(parameter);

			var sqlGeneratorResult = new SqlGeneratorResult(command);

			Assert.That(sqlGeneratorResult.SqlString, Is.EqualTo(commandText));
			Assert.That(sqlGeneratorResult.Parameters.Count, Is.EqualTo(1));
			Assert.That(sqlGeneratorResult.Parameters.First(), Is.EqualTo(parameter));
		}

		[Test]
		public void Constructor_PropertyAccess_UpdatesPropertyValues()
		{
			const string commandText = "CommandText";
			var parameter = new SqlParameter("parameterName1", "value1");

			var sqlGeneratorResult = new SqlGeneratorResult
			{
				SqlString = commandText
			};
			sqlGeneratorResult.Parameters.Add(parameter);

			Assert.That(sqlGeneratorResult.SqlString, Is.EqualTo(commandText));
			Assert.That(sqlGeneratorResult.Parameters.Count, Is.EqualTo(1));
			Assert.That(sqlGeneratorResult.Parameters.First(), Is.EqualTo(parameter));
		}
	}
}
