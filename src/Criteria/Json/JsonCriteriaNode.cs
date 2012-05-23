using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Criteria.Json
{
	public class JsonCriteriaNode : ICriteriaLeaf, ICompositeCriteria
	{
		[JsonProperty("operator")]
		public virtual Operator Operator { get; set; }

		// ICompositeCriteria members
		[JsonProperty("operands")]
		public IList<JsonCriteriaNode> Operands { get; set; }

		// ICriteriaLeaf members
		[JsonProperty("key")]
		public string Key { get; set; }

		[JsonProperty("value")]
		public object Value { get; set; }

		public override string ToString()
		{
			var operatorString = String.Format("{0}", Operator);

			switch (Operator)
			{
				case Operator.And:
					operatorString = "&&";
					break;
				case Operator.Or:
					operatorString = "||";
					break;
				case Operator.Equal:
					operatorString = "==";
					break;
				case Operator.NotEqual:
					operatorString = "!=";
					break;
				case Operator.LessThan:
					operatorString = "<";
					break;
				case Operator.LessThanOrEqual:
					operatorString = "<=";
					break;
				case Operator.GreaterThan:
					operatorString = ">";
					break;
				case Operator.GreaterThanOrEqual:
					operatorString = ">=";
					break;
				case Operator.IsIn:
					operatorString = "⊆";
					break;
			}

			string result;

			if(Operator.IsCompositeOperator())
			{
				result = String.Format("({0})", String.Join(String.Format(" {0} ", operatorString), Operands.Select(x => x.ToString())));
			}
			else if((Operator == Operator.IsFalse) || (Operator == Operator.IsTrue) || (Operator == Operator.IsSpecified) || (Operator == Operator.IsNotSpecified))
			{
				result = String.Format("({0} {1})", Key, operatorString);
			}
			else
			{
				var valueType = Value.GetType();

				var stringValue = ((!typeof(string).IsAssignableFrom(valueType)) && (!valueType.IsAssignableFrom(typeof(string))) && (typeof(IEnumerable).IsAssignableFrom(valueType) || (valueType.IsAssignableFrom(typeof(IEnumerable)))))
					? String.Format("[{0}]", String.Join(", ", ((IEnumerable)Value).Cast<object>().Select(x => String.Format("{0}", x))))
					: String.Format("{0}", Value);

				result = String.Format("({0} {1} {2})", Key, operatorString, stringValue);
			}

			return result;
		}
	}
}
