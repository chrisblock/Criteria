using System;
using System.Collections.Generic;

namespace Criteria.NHibernateCompatabilityTests.TreeModel
{
	public class ThreeLevelEntity
	{
		public virtual RootEntity RootEntityParent { get; set; }

		public virtual Guid ThreeLevelEntityId { get; set; }

		public virtual string ThreeLevelEntityField { get; set; }

		public virtual ICollection<ThreeLevelEntityChild> ThreeLevelEntityChildren { get; set; }

		public ThreeLevelEntity()
		{
			ThreeLevelEntityChildren = new List<ThreeLevelEntityChild>();
		}
	}
}
