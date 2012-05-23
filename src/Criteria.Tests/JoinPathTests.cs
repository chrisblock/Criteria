using Criteria.Registries;
using Criteria.Tests.TestModel;

using NUnit.Framework;

namespace Criteria.Tests
{
	[TestFixture]
	public class JoinPathTests
	{
		[Test]
		public void Equals_Null_IsFalse()
		{
			var instance = JoinPath.Create((TestEntityClass e) => e.TestIntegerProperty, (TestEntityClass e) => e.TestIntegerProperty);
			JoinPath instance2 = null;

			Assert.That(instance.Equals(instance2), Is.False);
		}

		[Test]
		public void Equals_SameInstance_IsTrue()
		{
			var instance = JoinPath.Create((TestEntityClass e) => e.TestIntegerProperty, (TestEntityClass e) => e.TestIntegerProperty);
			var instance2 = instance;

			Assert.That(instance.Equals(instance2), Is.True);
		}

		[Test]
		public void Equals_SameValue_IsTrue()
		{
			var instance = JoinPath.Create((TestEntityClass e) => e.TestIntegerProperty, (TestEntityClass e) => e.TestIntegerProperty);
			var instance2 = JoinPath.Create((TestEntityClass e) => e.TestIntegerProperty, (TestEntityClass e) => e.TestIntegerProperty);

			Assert.That(instance.Equals(instance2), Is.True);
		}

		[Test]
		public void Equals_DifferentStart_IsTrue()
		{
			var instance = JoinPath.Create((TestEntityTwo e) => e.TestEntityTwoId, (TestEntityClass e) => e.TestIntegerProperty);
			var instance2 = JoinPath.Create((TestEntityClass e) => e.TestIntegerProperty, (TestEntityClass e) => e.TestIntegerProperty);

			Assert.That(instance.Equals(instance2), Is.False);
		}

		[Test]
		public void Equals_DifferentEnd_IsTrue()
		{
			var instance = JoinPath.Create((TestEntityClass e) => e.TestIntegerProperty, (TestEntityTwo e) => e.TestEntityTwoId);
			var instance2 = JoinPath.Create((TestEntityClass e) => e.TestIntegerProperty, (TestEntityClass e) => e.TestIntegerProperty);

			Assert.That(instance.Equals(instance2), Is.False);
		}

		[Test]
		public void Equals_DifferentIsOneToMany_IsTrue()
		{
			var instance = JoinPath.Create((TestEntityClass e) => e.TestIntegerProperty, (TestEntityClass e) => e.TestIntegerProperty);
			var instance2 = JoinPath.Create((TestEntityClass e) => e.TestIntegerProperty, (TestEntityClass e) => e.TestIntegerProperty, true);

			Assert.That(instance.Equals(instance2), Is.False);
		}

		[Test]
		public void Object_Equals_Null_IsFalse()
		{
			object instance = JoinPath.Create((TestEntityClass e) => e.TestIntegerProperty, (TestEntityClass e) => e.TestIntegerProperty);
			object instance2 = null;

			Assert.That(instance.Equals(instance2), Is.False);
		}

		[Test]
		public void Object_Equals_SameInstance_IsTrue()
		{
			object instance = JoinPath.Create((TestEntityClass e) => e.TestIntegerProperty, (TestEntityClass e) => e.TestIntegerProperty);
			object instance2 = instance;

			Assert.That(instance.Equals(instance2), Is.True);
		}

		[Test]
		public void Object_Equals_SameValue_IsTrue()
		{
			object instance = JoinPath.Create((TestEntityClass e) => e.TestIntegerProperty, (TestEntityClass e) => e.TestIntegerProperty);
			object instance2 = JoinPath.Create((TestEntityClass e) => e.TestIntegerProperty, (TestEntityClass e) => e.TestIntegerProperty);

			Assert.That(instance.Equals(instance2), Is.True);
		}
	}
}
