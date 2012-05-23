using System;

namespace Criteria.NHibernateCompatabilityTests.TreeModel
{
	public class OneLevelEntity
	{
		public virtual RootEntity RootEntityParent { get; set; }

		public virtual Guid OneLevelEntityId { get; set; }

		public virtual string OneLevelEntityField { get; set; }
	}
}
