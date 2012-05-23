using System;
using System.Collections.Generic;

namespace Criteria.NHibernateCompatabilityTests.TreeModel
{
	public class TwoLevelEntity
	{
		public virtual RootEntity RootEntityParent { get; set; }

		public virtual Guid TwoLevelEntityId { get; set; }

		public virtual string TwoLevelEntityField { get; set; }

		public virtual ICollection<TwoLevelEntityChild> TwoLevelEntityChildren { get; set; }

		public TwoLevelEntity()
		{
			TwoLevelEntityChildren = new List<TwoLevelEntityChild>();
		}
	}
}
