using System;

namespace Criteria.NHibernate.IntegrationTests.TreeModel
{
	public class TwoLevelEntityChild
	{
		public virtual TwoLevelEntity TwoLevelEntityParent { get; set; }

		public virtual Guid TwoLevelEntityChildId { get; set; }

		public virtual string TwoLevelEntityChildField { get; set; }
	}
}
