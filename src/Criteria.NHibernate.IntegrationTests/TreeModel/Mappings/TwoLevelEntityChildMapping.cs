using System;

using FluentNHibernate.Mapping;

namespace Criteria.NHibernateCompatabilityTests.TreeModel.Mappings
{
	public class TwoLevelEntityChildMapping : ClassMap<TwoLevelEntityChild>
	{
		public TwoLevelEntityChildMapping()
		{
			Table("TwoLevelEntityChild");

			Id(x => x.TwoLevelEntityChildId, "TwoLevelEntityChildId")
				.GeneratedBy.Guid()
				.UnsavedValue(Guid.Empty);

			Map(x => x.TwoLevelEntityChildField, "TwoLevelEntityChildField");

			References(x => x.TwoLevelEntityParent, "TwoLevelEntityId")
				.Nullable();
		}
	}
}
