using System.Linq.Expressions;

namespace Criteria.Sql
{
	public class SqlGeneratorConfiguration
	{
		public bool Distinct { get; set; }
		public bool Count { get; set; }
		public bool Unalias { get; set; }
		public bool StarSelect { get; set; }
		public LambdaExpression DistinctCountPropertyExpression { get; set; }
	}
}
