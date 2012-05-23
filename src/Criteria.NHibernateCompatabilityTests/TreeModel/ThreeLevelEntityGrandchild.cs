using System;

namespace Criteria.NHibernateCompatabilityTests.TreeModel
{
	public class ThreeLevelEntityGrandchild
	{
		public virtual ThreeLevelEntityChild ThreeLevelEntityChildParent { get; set; }

		public virtual Guid ThreeLevelEntityGrandchildId { get; set; }

		public virtual string ThreeLevelEntityGrandchildField { get; set; }
	}
}
