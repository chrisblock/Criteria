using System;

using FluentNHibernate.Mapping;

namespace Criteria.NHibernateCompatabilityTests.TreeModel.Mappings
{
	public class RootEntityMapping : ClassMap<RootEntity>
	{
		public RootEntityMapping()
		{
			Table("RootEntity");

			Id(x => x.RootEntityId, "RootEntityId")
				.GeneratedBy.Guid()
				.UnsavedValue(Guid.Empty);

			Map(x => x.RootEntityField, "RootEntityField");
			Map(x => x.RootEntityIntegerField, "RootEntityIntegerField");
			Map(x => x.RootEntityBooleanField, "RootEntityBooleanField");

			HasMany(x => x.OneLevelEntities)
				.KeyColumn("RootEntityId")
				.Cascade.AllDeleteOrphan();

			HasMany(x => x.TwoLevelEntities)
				.KeyColumn("RootEntityId")
				.Cascade.AllDeleteOrphan();

			HasMany(x => x.ThreeLevelEntities)
				.KeyColumn("RootEntityId")
				.Cascade.AllDeleteOrphan();
		}
	}
}
