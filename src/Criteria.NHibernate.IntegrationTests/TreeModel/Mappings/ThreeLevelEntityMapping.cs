using System;

using FluentNHibernate.Mapping;

namespace Criteria.NHibernate.IntegrationTests.TreeModel.Mappings
{
	public class ThreeLevelEntityMapping : ClassMap<ThreeLevelEntity>
	{
		public ThreeLevelEntityMapping()
		{
			Table("ThreeLevelEntity");

			Id(x => x.ThreeLevelEntityId, "ThreeLevelEntityId")
				.GeneratedBy.Guid()
				.UnsavedValue(Guid.Empty);

			Map(x => x.ThreeLevelEntityField, "ThreeLevelEntityField");

			References(x => x.RootEntityParent, "RootEntityId")
				.Nullable();

			HasMany(x => x.ThreeLevelEntityChildren)
				.KeyColumn("ThreeLevelEntityId")
				.Cascade.AllDeleteOrphan();
		}
	}
}
