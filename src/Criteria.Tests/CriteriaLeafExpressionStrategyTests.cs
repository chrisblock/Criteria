// ReSharper disable InconsistentNaming

using System;
using System.Linq;
using System.Linq.Expressions;

using Criteria.Expressions;
using Criteria.Expressions.Impl;
using Criteria.Json;
using Criteria.Tests.Mocks;
using Criteria.Tests.TestModel;

using Newtonsoft.Json;

using NUnit.Framework;

namespace Criteria.Tests
{
	[TestFixture]
	public class CriteriaLeafExpressionStrategyTests
	{
		private ICriteriaLeafExpressionStrategy _expressionStrategy;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_expressionStrategy = new CriteriaLeafExpressionStrategy(new MockCriteriaTypeRegistry());
		}

		[Test]
		public void GetExpression_CriteriaLeafWithEqualsOperator_EqualsExpression()
		{
			var testPropertyName = "TestStringProperty";
			var testOperator = (int)Operator.Equal;
			var testValue = "TestValue";
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof (TestEntityClass), "c");

			var expected = Expression.Equal(Expression.Property(parameterExpression, testPropertyName), Expression.Constant(testValue));

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithNotEqualsOperator_NotEqualsExpression()
		{
			var testPropertyName = "TestStringProperty";
			var testOperator = (int)Operator.NotEqual;
			var testValue = "TestValue";
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var expected = Expression.NotEqual(Expression.Property(parameterExpression, testPropertyName), Expression.Constant(testValue));

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithLessThanOperator_LessThanExpression()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.LessThan;
			var testValue = 12;
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var expected = Expression.LessThan(Expression.Property(parameterExpression, testPropertyName), Expression.Constant(testValue, typeof(int)));

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithLessThanOrEqualToOperator_LessThanOrEqualExpression()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.LessThanOrEqual;
			var testValue = 12;
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var expected = Expression.LessThanOrEqual(Expression.Property(parameterExpression, testPropertyName), Expression.Constant(testValue, typeof(int)));

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithGreaterThanOperator_GreaterThanExpression()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.GreaterThan;
			var testValue = 12;
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var expected = Expression.GreaterThan(Expression.Property(parameterExpression, testPropertyName), Expression.Constant(testValue, typeof(int)));

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithGreaterThanOrEqualToOperator_GreaterThanOrEqualExpression()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.GreaterThanOrEqual;
			var testValue = 12;
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var expected = Expression.GreaterThanOrEqual(Expression.Property(parameterExpression, testPropertyName), Expression.Constant(testValue, typeof(int)));

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsInOperator_NewArrayContainsExpression()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.IsIn;
			var valueArray = new[] {12, 18};
			var testValue = String.Format("[{0}]", String.Join(",", valueArray));
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var arrayInit = Expression.NewArrayInit(typeof (int), valueArray.Select(x => Expression.Constant(x, x.GetType())));

			var expected = Expression.Call(typeof(Enumerable), "Contains", new[] { typeof(int) }, new Expression[] { arrayInit, Expression.Property(parameterExpression, testPropertyName) });

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsNotInOperator_NewArrayContainsExpression()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.IsNotIn;
			var valueArray = new[] { 12, 18 };
			var testValue = String.Format("[{0}]", String.Join(",", valueArray));
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var arrayInit = Expression.NewArrayInit(typeof(int), valueArray.Select(x => Expression.Constant(x, x.GetType())));

			var expected = Expression.Not(Expression.Call(typeof(Enumerable), "Contains", new[] { typeof(int) }, new Expression[] { arrayInit, Expression.Property(parameterExpression, testPropertyName) }));

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithContainsOperator_ContainsExpression()
		{
			var testPropertyName = "TestStringProperty";
			var testOperator = (int)Operator.Contains;
			var testValue = "Contains";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var expected = Expression.Call(Expression.Property(parameterExpression, testPropertyName), "Contains", new Type[0], new Expression[] { Expression.Constant(testValue, typeof(string)) });

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithContainsInvalidOperator_ThrowsArgumentException()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = 99;
			var testValue = "Contains";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Assert.That(() => _expressionStrategy.GetExpression(criteriaLeaf), Throws.ArgumentException);
		}

		[Test]
		public void GetExpression_CriteriaLeafWithContainsOperatorAndInvalidTypeParameter_ThrowsArgumentException()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.Contains;
			var testValue = "Contains";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Assert.That(() => _expressionStrategy.GetExpression(criteriaLeaf), Throws.ArgumentException);
		}

		[Test]
		public void GetExpression_CriteriaLeafWithDoesNotContainsOperator_ContainsExpression()
		{
			var testPropertyName = "TestStringProperty";
			var testOperator = (int)Operator.DoesNotContain;
			var testValue = "Contains";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var expected = Expression.Not(Expression.Call(Expression.Property(parameterExpression, testPropertyName), "Contains", new Type[0], new Expression[] { Expression.Constant(testValue, typeof(string)) }));

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithDoesNotContainsOperatorAndInvalidTypeParameter_ThrowsArgumentException()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.DoesNotContain;
			var testValue = "Contains";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Assert.That(() => _expressionStrategy.GetExpression(criteriaLeaf), Throws.ArgumentException);
		}

		[Test]
		public void GetExpression_CriteriaLeafWithStartsWithOperator_StartsWithExpression()
		{
			var testPropertyName = "TestStringProperty";
			var testOperator = (int)Operator.StartsWith;
			var testValue = "StartsWith";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var expected = Expression.Call(Expression.Property(parameterExpression, testPropertyName), "StartsWith", new Type[0], new Expression[] { Expression.Constant(testValue, typeof(string)) });

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithStartsWithOperatorAndInvalidTypeParameter_ThrowsArgumentException()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.StartsWith;
			var testValue = "StartsWith";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Assert.That(() => _expressionStrategy.GetExpression(criteriaLeaf), Throws.ArgumentException);
		}

		[Test]
		public void GetExpression_CriteriaLeafWithDoesNotStartWithOperator_StartsWithExpression()
		{
			var testPropertyName = "TestStringProperty";
			var testOperator = (int)Operator.DoesNotStartWith;
			var testValue = "StartsWith";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var expected = Expression.Not(Expression.Call(Expression.Property(parameterExpression, testPropertyName), "StartsWith", new Type[0], new Expression[] { Expression.Constant(testValue, typeof(string)) }));

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithDoesNotStartWithOperatorAndInvalidTypeParameter_ThrowsArgumentException()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.DoesNotStartWith;
			var testValue = "StartsWith";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Assert.That(() => _expressionStrategy.GetExpression(criteriaLeaf), Throws.ArgumentException);
		}

		[Test]
		public void GetExpression_CriteriaLeafWithEndsWithOperator_EndsWithExpression()
		{
			var testPropertyName = "TestStringProperty";
			var testOperator = (int)Operator.EndsWith;
			var testValue = "EndsWith";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var expected = Expression.Call(Expression.Property(parameterExpression, testPropertyName), "EndsWith", new Type[0], new Expression[] { Expression.Constant(testValue, typeof(string)) });

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithEndsWithOperatorAndInvalidTypeParameter_ThrowsArgumentException()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.EndsWith;
			var testValue = "EndsWith";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Assert.That(() => _expressionStrategy.GetExpression(criteriaLeaf), Throws.ArgumentException);
		}

		[Test]
		public void GetExpression_CriteriaLeafWithDoesNotEndWithOperator_EndsWithExpression()
		{
			var testPropertyName = "TestStringProperty";
			var testOperator = (int)Operator.DoesNotEndWith;
			var testValue = "EndsWith";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Expression expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var expected = Expression.Not(Expression.Call(Expression.Property(parameterExpression, testPropertyName), "EndsWith", new Type[0], new Expression[] { Expression.Constant(testValue, typeof(string)) }));

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithDoesNotEndWithOperatorAndInvalidTypeParameter_ThrowsArgumentException()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.DoesNotEndWith;
			var testValue = "EndsWith";

			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Assert.That(() => _expressionStrategy.GetExpression(criteriaLeaf), Throws.ArgumentException);
		}

		[Test]
		public void GetExpression_CriteriaLeafWithBetweenOperator_BetweenExpression()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.Between;
			var testValue = String.Format("[{0}]", String.Join(",", new[] { 2, 5 }));
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof (TestEntityClass), "c");
			var propertyExpression = Expression.Property(parameterExpression, testPropertyName);

			var expected = Expression.AndAlso(Expression.GreaterThanOrEqual(propertyExpression, Expression.Constant(2)),
				Expression.LessThanOrEqual(propertyExpression, Expression.Constant(5)));

			Assert.That(expr.ToString(), Is.EqualTo(expected.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithBetweenOperatorOnInvalidValues_ThrowsArgumentException()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.Between;
			var testValue = String.Format("[{0}]", String.Join(",", new[] { 2 }));
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: {2}}}", testPropertyName, testOperator, testValue);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Assert.That(() => { _expressionStrategy.GetExpression(criteriaLeaf); }, Throws.ArgumentException);
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsTrueOperatorOnNonBooleanProperty_ThrowsException()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.IsTrue;
			var jsonString = String.Format("{{key: '{0}', operator: {1}}}", testPropertyName, testOperator);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Assert.That(() => { _expressionStrategy.GetExpression(criteriaLeaf); }, Throws.ArgumentException);
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsTrueOperator_PropertyExpression()
		{
			var testPropertyName = "TestBooleanProperty";
			var testOperator = (int)Operator.IsTrue;
			var jsonString = String.Format("{{key: '{0}', operator: {1}}}", testPropertyName, testOperator);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");
			var propertyExpression = Expression.Property(parameterExpression, testPropertyName);

			var expected = propertyExpression;

			Assert.That(expr.ToString(), Is.EqualTo(expected.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsFalseOperatorOnNonBooleanProperty_ThrowsException()
		{
			var testPropertyName = "TestIntegerProperty";
			var testOperator = (int)Operator.IsFalse;
			var jsonString = String.Format("{{key: '{0}', operator: {1}}}", testPropertyName, testOperator);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			Assert.That(() => { _expressionStrategy.GetExpression(criteriaLeaf); }, Throws.ArgumentException);
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsFalseOperator_NotPropertyExpression()
		{
			var testPropertyName = "TestBooleanProperty";
			var testOperator = (int)Operator.IsFalse;
			var jsonString = String.Format("{{key: '{0}', operator: {1}}}", testPropertyName, testOperator);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");
			var propertyExpression = Expression.Property(parameterExpression, testPropertyName);

			var expected = Expression.Not(propertyExpression);

			Assert.That(expr.ToString(), Is.EqualTo(expected.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsSpecifiedOperatorAndDecimalTypeOperand_DoesNotEqualNullExpression()
		{
			var testPropertyName = "TestDecimalProperty";
			var testOperator = (int)Operator.IsSpecified;
			var jsonString = String.Format("{{key: '{0}', operator: {1}}}", testPropertyName, testOperator);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");
			var propertyExpression = Expression.Property(parameterExpression, testPropertyName);

			var expected = Expression.NotEqual(propertyExpression, Expression.Constant(null));

			Assert.That(expr.ToString(), Is.EqualTo(expected.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsSpecifiedOperatorAndStringTypeOperand_DoesNotEqualNullAndDoesNotEqualEmptyString()
		{
			var testPropertyName = "TestStringProperty";
			var testOperator = (int)Operator.IsSpecified;
			var jsonString = String.Format("{{key: '{0}', operator: {1}}}", testPropertyName, testOperator);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");
			var propertyExpression = Expression.Property(parameterExpression, testPropertyName);

			var doesNotEqualNull = Expression.NotEqual(propertyExpression, Expression.Constant(null, typeof(string)));
			var doesNotEqualEmptyString = Expression.NotEqual(propertyExpression, Expression.Constant(String.Empty, typeof(string)));

			var expected = Expression.AndAlso(doesNotEqualNull, doesNotEqualEmptyString);

			Assert.That(expr.ToString(), Is.EqualTo(expected.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsNotSpecifiedOperatorAndDecimalTypeOperand_EqualsNull()
		{
			var testPropertyName = "TestDecimalProperty";
			var testOperator = (int)Operator.IsNotSpecified;
			var jsonString = String.Format("{{key: '{0}', operator: {1}}}", testPropertyName, testOperator);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");
			var propertyExpression = Expression.Property(parameterExpression, testPropertyName);

			var expected = Expression.Equal(propertyExpression, Expression.Constant(null));

			Assert.That(expr.ToString(), Is.EqualTo(expected.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithIsNotSpecifiedOperatorAndStringTypeOperand_EqualsNullOrEqualsEmptyString()
		{
			var testPropertyName = "TestStringProperty";
			var testOperator = (int)Operator.IsNotSpecified;
			var jsonString = String.Format("{{key: '{0}', operator: {1}}}", testPropertyName, testOperator);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");
			var propertyExpression = Expression.Property(parameterExpression, testPropertyName);

			var equalsNull = Expression.Equal(propertyExpression, Expression.Constant(null, typeof(string)));
			var equalsEmptyString = Expression.Equal(propertyExpression, Expression.Constant(String.Empty, typeof(string)));

			var expected = Expression.OrElse(equalsNull, equalsEmptyString);

			Assert.That(expr.ToString(), Is.EqualTo(expected.ToString()));
		}

		[Test]
		public void GetExpression_CriteriaLeafWithEqualsColumnOperator_EqualsColumnExpression()
		{
			var testPropertyName = "TestStringProperty";
			var testOperator = (int)Operator.EqualsColumn;
			var otherColumn = "TestStringProperty";
			var jsonString = String.Format("{{key: '{0}', operator: {1}, value: '{2}'}}", testPropertyName, testOperator, otherColumn);

			ICriteriaLeaf criteriaLeaf = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var expr = _expressionStrategy.GetExpression(criteriaLeaf);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");
			var propertyExpression = Expression.Property(parameterExpression, testPropertyName);
			var otherColumnPropertyExpression = Expression.Property(parameterExpression, otherColumn);

			var expected = Expression.Equal(propertyExpression, otherColumnPropertyExpression);

			Assert.That(expr.ToString(), Is.EqualTo(expected.ToString()));
		}
	}
}
