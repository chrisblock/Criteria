using System.Linq.Expressions;

namespace Criteria.Sql
{
	public interface ISqlGenerator
	{
		SqlGeneratorResult Generate(Expression expression);
		SqlGeneratorResult GenerateUnaliased(Expression expression);
		SqlGeneratorResult GenerateStarSelect(Expression expression);
	}
}
