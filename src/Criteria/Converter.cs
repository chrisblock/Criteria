using System;
using System.Linq;

namespace Criteria
{
	public static class Converter
	{
		public static object NullableSafeChangeType(object value, Type conversionType)
		{
			var targetType = conversionType;

			if(targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				targetType = conversionType.GetGenericArguments().Single();
			}

			return Convert.ChangeType(value, targetType);
		}
	}
}
