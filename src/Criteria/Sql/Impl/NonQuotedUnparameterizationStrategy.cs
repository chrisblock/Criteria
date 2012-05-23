using System.Data;

namespace Criteria.Sql.Impl
{
	public class NonQuotedUnparameterizationStrategy : ISqlUnparameterizationStrategy
	{
		public string Unparameterize(IDataParameter parameter)
		{
			return parameter.Value.ToString();
		}
	}
}
