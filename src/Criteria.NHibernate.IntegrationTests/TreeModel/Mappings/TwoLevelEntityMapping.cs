using System;

using FluentNHibernate.Mapping;

namespace Criteria.NHibernate.IntegrationTests.TreeModel.Mappings
{
	public class TwoLevelEntityMapping : ClassMap<TwoLevelEntity>
	{
		public TwoLevelEntityMapping()
		{
			Table("TwoLevelEntity");

			Id(x => x.TwoLevelEntityId, "TwoLevelEntityId")
				.GeneratedBy.Guid()
				.UnsavedValue(Guid.Empty);

			Map(x => x.TwoLevelEntityField, "TwoLevelEntityField");

			References(x => x.RootEntityParent, "RootEntityId")
				.Nullable();

			HasMany(x => x.TwoLevelEntityChildren)
				.KeyColumn("TwoLevelEntityId")
				.Cascade.AllDeleteOrphan();
		}
	}
}
