using System;

using FluentNHibernate.Mapping;

namespace Criteria.NHibernate.IntegrationTests.TreeModel.Mappings
{
	public class ThreeLevelEntityChildMapping : ClassMap<ThreeLevelEntityChild>
	{
		public ThreeLevelEntityChildMapping()
		{
			Table("ThreeLevelEntityChild");

			Id(x => x.ThreeLevelEntityChildId, "ThreeLevelEntityChildId")
				.GeneratedBy.Guid()
				.UnsavedValue(Guid.Empty);

			Map(x => x.ThreeLevelEntityChildField, "ThreeLevelEntityChildField");

			References(x => x.ThreeLevelEntityParent, "ThreeLevelEntityId")
				.Nullable();

			HasMany(x => x.ThreeLevelEntityGrandchildren)
				.KeyColumn("ThreeLevelEntityChildId")
				.Cascade.AllDeleteOrphan();
		}
	}
}
