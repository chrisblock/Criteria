using System;
using System.Data;

namespace Criteria.Sql.Impl
{
	public class EscapedStringUnparameterizationStrategy : ISqlUnparameterizationStrategy
	{
		public string Unparameterize(IDataParameter parameter)
		{
			var escapedResult = EscapeString(parameter.Value.ToString());
			return String.Format("'{0}'", escapedResult);
		}

		private static string EscapeString(string value)
		{
			// TODO: Get this regex working to avoid SQL injection
			//return Regex.Replace(value, @"([^\w\s])", @"\$1", RegexOptions.Multiline);
			return value.Replace("'", "''");
		}
	}
}
