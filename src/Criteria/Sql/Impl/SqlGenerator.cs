using System;
using System.Linq.Expressions;

namespace Criteria.Sql.Impl
{
	public class SqlGenerator : ISqlGenerator
	{
		public SqlGeneratorResult Generate(Expression expression)
		{
			throw new NotImplementedException("No ISqlGenerator::Generate implemented.");
		}

		public SqlGeneratorResult GenerateUnaliased(Expression expression)
		{
			throw new NotImplementedException("No ISqlGenerator::GenerateUnaliased implemented.");
		}

		public SqlGeneratorResult GenerateStarSelect(Expression expression)
		{
			throw new NotImplementedException("No ISqlGenerator::GenerateStarSelect implemented.");
		}
	}
}
