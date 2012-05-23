namespace Criteria.Json
{
	public interface ICriteriaLeaf
	{
		Operator Operator { get; set; }

		string Key { get; set; }
		object Value { get; set; }
	}
}
