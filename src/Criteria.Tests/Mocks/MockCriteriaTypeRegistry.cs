using Criteria.Registries.Impl;
using Criteria.Tests.LinqToObjectsModel;
using Criteria.Tests.TestModel;

namespace Criteria.Tests.Mocks
{
	public class MockCriteriaTypeRegistry : BaseCriteriaTypeRegistry
	{
		public MockCriteriaTypeRegistry()
		{
			RegisterCriteriaKey("TestStringProperty", (TestEntityClass c) => c.TestStringProperty);
			RegisterCriteriaKey("TestIntegerProperty", (TestEntityClass c) => c.TestIntegerProperty);
			RegisterCriteriaKey("TestBooleanProperty", (TestEntityClass c) => c.TestBooleanProperty);
			RegisterCriteriaKey("TestDecimalProperty", (TestEntityClass c) => c.TestDecimalProperty);

			RegisterCriteriaKey("LinqToObjectsOneField", (LinqToObjectsOne o) => o.LinqToObjectsOneField);
			RegisterCriteriaKey("LinqToObjectsTwoField", (LinqToObjectsTwo o) => o.LinqToObjectsTwoField);
			RegisterCriteriaKey("LinqToObjectsThreeField", (LinqToObjectsThree o) => o.LinqToObjectsThreeField);
		}
	}
}
