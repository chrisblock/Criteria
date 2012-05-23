using Criteria.Registries.Impl;

namespace Criteria.NHibernateCompatabilityTests.TreeModel.Registries
{
	public class TreeModelJoinPathRegistry : BaseJoinPathRegistry
	{
		public TreeModelJoinPathRegistry()
		{
			RegisterOneTimeJoinFor<RootEntity>(join => join.To<OneLevelEntity>().On(root => root.RootEntityId, one => one.RootEntityParent.RootEntityId));

			RegisterOneTimeJoinFor<RootEntity>(join => join.To<TwoLevelEntity>().On(root => root.RootEntityId, two => two.RootEntityParent.RootEntityId));

			RegisterOneTimeJoinFor<TwoLevelEntity>(join => join.To<TwoLevelEntityChild>().On(two => two.TwoLevelEntityId, child => child.TwoLevelEntityParent.TwoLevelEntityId));
			
			RegisterOneTimeJoinFor<RootEntity>(join => join.To<ThreeLevelEntity>().On(root => root.RootEntityId, three => three.RootEntityParent.RootEntityId));

			RegisterOneTimeJoinFor<ThreeLevelEntity>(join => join.To<ThreeLevelEntityChild>().On(three => three.ThreeLevelEntityId, child => child.ThreeLevelEntityParent.ThreeLevelEntityId));

			RegisterOneTimeJoinFor<ThreeLevelEntityChild>(join => join.To<ThreeLevelEntityGrandchild>().On(child => child.ThreeLevelEntityChildId, grandchild => grandchild.ThreeLevelEntityChildParent.ThreeLevelEntityChildId));
		}
	}
}
