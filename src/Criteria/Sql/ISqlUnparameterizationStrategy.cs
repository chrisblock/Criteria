using System.Data;

namespace Criteria.Sql
{
	public interface ISqlUnparameterizationStrategy
	{
		string Unparameterize(IDataParameter parameter);
	}
}
