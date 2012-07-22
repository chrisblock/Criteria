// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;

using Criteria.Json;

using NUnit.Framework;

namespace Criteria.Tests
{
	[TestFixture]
	public class JsonCriteriaNodeTests
	{
		[Test]
		public void ToString_OperatorAndWithoutOperands_OutputsString()
		{
			var node = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>()
			};

			Assert.That(node.ToString(), Is.EqualTo("()"));
		}

		[Test]
		public void ToString_OperatorAndWithOperands_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.And,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Operator = Operator.Equal,
						Key = key,
						Value = value
					},
					new JsonCriteriaNode
					{
						Operator = Operator.NotEqual,
						Key = key,
						Value = value
					},
				}
			};

			Assert.That(node.ToString(), Is.EqualTo("((TestKey == TestValue) && (TestKey != TestValue))"));
		}

		[Test]
		public void ToString_OperatorOrWithoutOperands_OutputsString()
		{
			var node = new JsonCriteriaNode
			{
				Operator = Operator.Or,
				Operands = new List<JsonCriteriaNode>()
			};

			Assert.That(node.ToString(), Is.EqualTo("()"));
		}

		[Test]
		public void ToString_OperatorOrWithOperands_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.Or,
				Operands = new List<JsonCriteriaNode>
				{
					new JsonCriteriaNode
					{
						Operator = Operator.Equal,
						Key = key,
						Value = value
					},
					new JsonCriteriaNode
					{
						Operator = Operator.NotEqual,
						Key = key,
						Value = value
					},
				}
			};

			Assert.That(node.ToString(), Is.EqualTo("((TestKey == TestValue) || (TestKey != TestValue))"));
		}

		[Test]
		public void ToString_OperatorBetween_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.Between,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} Between {1})", key, value)));
		}

		[Test]
		public void ToString_OperatorContains_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.Contains,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} Contains {1})", key, value)));
		}

		[Test]
		public void ToString_OperatorDoesNotContain_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.DoesNotContain,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} DoesNotContain {1})", key, value)));
		}

		[Test]
		public void ToString_OperatorDoesNotEndWith_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.DoesNotEndWith,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} DoesNotEndWith {1})", key, value)));
		}

		[Test]
		public void ToString_OperatorDoesNotStartWith_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.DoesNotStartWith,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} DoesNotStartWith {1})", key, value)));
		}

		[Test]
		public void ToString_OperatorEndsWith_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.EndsWith,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} EndsWith {1})", key, value)));
		}

		[Test]
		public void ToString_OperatorEqual_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.Equal,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} == {1})", key, value)));
		}

		[Test]
		public void ToString_OperatorGreaterThan_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.GreaterThan,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} > {1})", key, value)));
		}

		[Test]
		public void ToString_OperatorGreaterThanOrEqual_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.GreaterThanOrEqual,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} >= {1})", key, value)));
		}

		[Test]
		public void ToString_OperatorIsTrue_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.IsTrue,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} IsTrue)", key)));
		}

		[Test]
		public void ToString_OperatorIsFalse_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.IsFalse,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} IsFalse)", key)));
		}

		[Test]
		public void ToString_OperatorIsSpecified_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.IsSpecified,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} IsSpecified)", key)));
		}

		[Test]
		public void ToString_OperatorIsNotSpecified_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.IsNotSpecified,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} IsNotSpecified)", key)));
		}

		[Test]
		public void ToString_OperatorIsIn_OutputsString()
		{
			const string key = "TestKey";
			var value = new[] {"TestValue", "OtherTestValue"};

			var node = new JsonCriteriaNode
			{
				Operator = Operator.IsIn,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} ⊆ [{1}, {2}])", key, value[0], value[1])));
		}

		[Test]
		public void ToString_OperatorIsNotIn_OutputsString()
		{
			const string key = "TestKey";
			var value = new[] { "TestValue", "OtherTestValue" };

			var node = new JsonCriteriaNode
			{
				Operator = Operator.IsNotIn,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} IsNotIn [{1}, {2}])", key, value[0], value[1])));
		}

		[Test]
		public void ToString_OperatorLessThan_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.LessThan,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} < {1})", key, value)));
		}

		[Test]
		public void ToString_OperatorLessThanOrEqual_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.LessThanOrEqual,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} <= {1})", key, value)));
		}

		[Test]
		public void ToString_OperatorStartsWith_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.StartsWith,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} StartsWith {1})", key, value)));
		}

		[Test]
		public void ToString_OperatorUnknown_OutputsString()
		{
			const string key = "TestKey";
			const string value = "TestValue";

			var node = new JsonCriteriaNode
			{
				Operator = Operator.Unknown,
				Key = key,
				Value = value
			};

			Assert.That(node.ToString(), Is.EqualTo(String.Format("({0} Unknown {1})", key, value)));
		}
	}
}
