using System;
using System.Collections.Generic;

namespace Criteria.NHibernateCompatabilityTests.TreeModel
{
	public class RootEntity
	{
		public virtual Guid RootEntityId { get; set; }

		public virtual string RootEntityField { get; set; }

		public virtual int RootEntityIntegerField { get; set; }

		public virtual bool RootEntityBooleanField { get; set; }

		public virtual ICollection<OneLevelEntity> OneLevelEntities { get; set; }

		public virtual ICollection<TwoLevelEntity> TwoLevelEntities { get; set; }
		public virtual ICollection<ThreeLevelEntity> ThreeLevelEntities { get; set; }

		public RootEntity()
		{
			OneLevelEntities = new List<OneLevelEntity>();
			TwoLevelEntities = new List<TwoLevelEntity>();
			ThreeLevelEntities = new List<ThreeLevelEntity>();
		}
	}
}
