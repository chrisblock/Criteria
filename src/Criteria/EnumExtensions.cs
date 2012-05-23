using Criteria.Json;

namespace Criteria
{
	public static class EnumExtensions
	{
		public static bool IsCompositeOperator(this Operator op)
		{
			var result = false;

			switch (op)
			{
				case Operator.And:
					result = true;
					break;
				case Operator.Or:
					result = true;
					break;
			}

			return result;
		}
	}
}
