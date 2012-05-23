using System;
using System.Linq.Expressions;

namespace Criteria.Registries
{
	public class TypeRegistryResult
	{
		public Type EntityType { get; set; }
		public Type PropertyType { get; set; }
		public Expression AccessorExpression { get; set; }
	}
}
