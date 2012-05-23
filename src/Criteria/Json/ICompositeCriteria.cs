using System.Collections.Generic;

namespace Criteria.Json
{
	public interface ICompositeCriteria
	{
		Operator Operator { get; set; }
		IList<JsonCriteriaNode> Operands { get; set; }
	}
}
