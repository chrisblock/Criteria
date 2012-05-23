using System;
using System.Linq.Expressions;

namespace Criteria.Sql
{
	public class SqlGeneratorConfigurator
	{
		private readonly SqlGeneratorConfiguration _config;

		public SqlGeneratorConfigurator(SqlGeneratorConfiguration config)
		{
			_config = config;
		}

		public SqlGeneratorConfigurator CountDistinctProperty<T, TResult>(Expression<Func<T, TResult>> lambda)
		{
			_config.DistinctCountPropertyExpression = lambda;
			return this;
		}

		public SqlGeneratorConfigurator MakeDistinct()
		{
			if(_config.Count)
			{
				throw new InvalidOperationException("Cannot GetCount when MakeDistinct already specified. To use Count and Distinct specify a CountDistinctProperty lambda instead.");
			}

			if (_config.StarSelect)
			{
				throw new InvalidOperationException("Cannot MakeDistinct when StarSelect already specified.");
			}

			_config.Distinct = true;
			return this;
		}

		public SqlGeneratorConfigurator GetCount()
		{
			if(_config.Distinct)
			{
				throw new InvalidOperationException("Cannot GetCount when MakeDistinct already specified. To use Count and Distinct specify a CountDistinctProperty lambda instead.");
			}
			
			if(_config.Unalias)
			{
				throw new InvalidOperationException("Cannot GetCount when Unalias already specified.");
			}

			if (_config.StarSelect)
			{
				throw new InvalidOperationException("Cannot GetCount when StarSelect already specified.");
			}

			_config.Count = true;
			return this;
		}

		public SqlGeneratorConfigurator Unalias()
		{
			if (_config.Count)
			{
				throw new InvalidOperationException("Cannot Unlaias when GetCount already specified.");
			}

			if (_config.StarSelect)
			{
				throw new InvalidOperationException("Cannot Unalias when StarSelect already specified.");
			}

			_config.Unalias = true;
			return this;
		}

		public SqlGeneratorConfigurator StarSelect()
		{
			if (_config.Count)
			{
				throw new InvalidOperationException("Cannot StarSelect when GetCount already specified.");
			}

			if (_config.Distinct)
			{
				throw new InvalidOperationException("Cannot StarSelect when MakeDistinct already specified.");
			}

			if (_config.Unalias)
			{
				throw new InvalidOperationException("Cannot StarSelect when Unalias already specified.");
			}

			_config.StarSelect = true;
			return this;
		}
	}
}
