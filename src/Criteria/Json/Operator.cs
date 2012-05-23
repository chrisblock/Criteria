namespace Criteria.Json
{
	public enum Operator
	{
		// Have to have this since 0 is falsy in JavaScript
		Unknown = 0,

		// Composite types; these operator types are only allowed on Composite nodes
		And,
		Or,

		// Leaf types; these operator types are only allowed on Leaf nodes
		Equal,
		NotEqual,
		LessThan,
		LessThanOrEqual,
		GreaterThan,
		GreaterThanOrEqual,
		Contains,
		DoesNotContain,
		Between,
		StartsWith,
		DoesNotStartWith,
		EndsWith,
		DoesNotEndWith,
		IsIn,
		IsNotIn,
		IsTrue,
		IsFalse,
		IsNotSpecified,
		EqualsColumn,
		IsSpecified,
		InLast30Days,
		InLast60Days,
		InLast90Days,
		InLast120Days,
		InLast180Days,
		InLastYear
	}
}
