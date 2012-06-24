using System;
using System.Collections.Generic;

namespace Criteria.NHibernate.IntegrationTests.TreeModel
{
	public class ThreeLevelEntityChild
	{
		public virtual ThreeLevelEntity ThreeLevelEntityParent { get; set; }

		public virtual Guid ThreeLevelEntityChildId { get; set; }

		public virtual ICollection<ThreeLevelEntityGrandchild> ThreeLevelEntityGrandchildren { get; set; }

		public virtual string ThreeLevelEntityChildField { get; set; }

		public ThreeLevelEntityChild()
		{
			ThreeLevelEntityGrandchildren = new List<ThreeLevelEntityGrandchild>();
		}
	}
}
