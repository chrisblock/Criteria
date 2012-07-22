// ReSharper disable InconsistentNaming

using System;
using System.Linq.Expressions;

using Criteria.Expressions;
using Criteria.Json;
using Criteria.Tests.Mocks;
using Criteria.Tests.TestModel;

using Newtonsoft.Json;

using NUnit.Framework;

namespace Criteria.Tests
{
	[TestFixture]
	public class ExpressionBuilderTests
	{
		private ExpressionBuilder _expressionBuilder;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_expressionBuilder = new ExpressionBuilder(new MockCriteriaTypeRegistry());
		}

		[Test]
		public void Build_SimpleAndCriteria_SimpleAndExpression()
		{
			var testCompositeOperator = (int)Operator.And;

			var testPropertyName = "TestStringProperty";
			var testOperator = (int)Operator.Equal;
			var testValue = "TestValue";
			var jsonString = String.Format("{{operator: {0}, operands: [{{key: '{1}', operator: {2}, value: '{3}'}}, {{key: '{1}', operator: {2}, value: '{3}'}}]}}", testCompositeOperator, testPropertyName, testOperator, testValue);

			var criteria = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var expr = _expressionBuilder.Build(criteria);

			var parameterExpression = Expression.Parameter(typeof (TestEntityClass), "c");

			var expected = Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, testPropertyName), Expression.Constant(testValue, testValue.GetType())),
												Expression.Equal(Expression.Property(parameterExpression, testPropertyName), Expression.Constant(testValue, testValue.GetType())));

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}

		[Test]
		public void Build_SimpleOrCriteria_SimpleOrExpression()
		{
			var testCompositeOperator = (int)Operator.Or;

			var testPropertyName = "TestStringProperty";
			var testOperator = (int)Operator.Equal;
			var testValue = "TestValue";
			var jsonString = String.Format("{{operator: {0}, operands: [{{key: '{1}', operator: {2}, value: '{3}'}}, {{key: '{1}', operator: {2}, value: '{3}'}}]}}", testCompositeOperator, testPropertyName, testOperator, testValue);

			var criteria = JsonConvert.DeserializeObject<JsonCriteriaNode>(jsonString);

			var expr = _expressionBuilder.Build(criteria);

			var parameterExpression = Expression.Parameter(typeof(TestEntityClass), "c");

			var expected = Expression.OrElse(Expression.Equal(Expression.Property(parameterExpression, testPropertyName), Expression.Constant(testValue, testValue.GetType())),
												Expression.Equal(Expression.Property(parameterExpression, testPropertyName), Expression.Constant(testValue, testValue.GetType())));

			Assert.That(expected.ToString(), Is.EqualTo(expr.ToString()));
		}
	}
}
