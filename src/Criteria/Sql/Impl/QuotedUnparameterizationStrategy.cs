using System;
using System.Data;

namespace Criteria.Sql.Impl
{
	public class QuotedUnparameterizationStrategy : ISqlUnparameterizationStrategy
	{
		public string Unparameterize(IDataParameter parameter)
		{
			return String.Format("'{0}'", parameter.Value);
		}
	}
}
