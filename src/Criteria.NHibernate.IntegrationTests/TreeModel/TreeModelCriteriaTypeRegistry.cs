using Criteria.Registries.Impl;

namespace Criteria.NHibernateCompatabilityTests.TreeModel
{
	public class TreeModelCriteriaTypeRegistry : BaseCriteriaTypeRegistry
	{
		public TreeModelCriteriaTypeRegistry()
		{
			RegisterCriteriaKey("RootEntityField", (RootEntity x) => x.RootEntityField);
			RegisterCriteriaKey("RootEntityIntegerField", (RootEntity x) => x.RootEntityIntegerField);
			RegisterCriteriaKey("RootEntityBooleanField", (RootEntity x) => x.RootEntityBooleanField);
			RegisterCriteriaKey("OneLevelEntityField", (OneLevelEntity x) => x.OneLevelEntityField);
			RegisterCriteriaKey("TwoLevelEntityField", (TwoLevelEntity x) => x.TwoLevelEntityField);
			RegisterCriteriaKey("TwoLevelEntityChildField", (TwoLevelEntityChild x) => x.TwoLevelEntityChildField);
			RegisterCriteriaKey("ThreeLevelEntityField", (ThreeLevelEntity x) => x.ThreeLevelEntityField);
			RegisterCriteriaKey("ThreeLevelEntityChildField", (ThreeLevelEntityChild x) => x.ThreeLevelEntityChildField);
			RegisterCriteriaKey("ThreeLevelEntityGrandchildField", (ThreeLevelEntityGrandchild x) => x.ThreeLevelEntityGrandchildField);
		}
	}
}
