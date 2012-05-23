using System;
using System.Data.SqlClient;

using Criteria.Sql.Impl;

using NUnit.Framework;

namespace Criteria.Tests
{
	[TestFixture]
	public class SqlUnparameterizationStrategyTests
	{
		[Test]
		public void NonQuotedUnparameterizationStrategy_UnparameterizesCorrectly()
		{
			const string parameterValue = "g'day";
			var parameter = new SqlParameter("parameterName", parameterValue);
			var strategy = new NonQuotedUnparameterizationStrategy();

			Assert.That(strategy.Unparameterize(parameter), Is.EqualTo(parameterValue));
		}

		[Test]
		public void QuotedUnparameterizationStrategy_UnparameterizesCorrectly()
		{
			const string parameterValue = "g'day";
			var parameter = new SqlParameter("parameterName", parameterValue);
			var strategy = new QuotedUnparameterizationStrategy();

			Assert.That(strategy.Unparameterize(parameter), Is.EqualTo(String.Format("'{0}'", parameterValue)));
		}

		[Test]
		public void EscapedUnparameterizationStrategy_UnparameterizesCorrectly()
		{
			const string parameterValue = "g'day";
			var parameter = new SqlParameter("parameterName", parameterValue);
			var strategy = new EscapedStringUnparameterizationStrategy();

			Assert.That(strategy.Unparameterize(parameter), Is.EqualTo(String.Format("'{0}'", parameterValue.Replace("'", "''"))));
		}
	}
}
