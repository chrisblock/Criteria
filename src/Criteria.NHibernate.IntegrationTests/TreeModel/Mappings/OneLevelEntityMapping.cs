using System;

using FluentNHibernate.Mapping;

namespace Criteria.NHibernate.IntegrationTests.TreeModel.Mappings
{
	public class OneLevelEntityMapping : ClassMap<OneLevelEntity>
	{
		public OneLevelEntityMapping()
		{
			Table("OneLevelEntity");

			Id(x => x.OneLevelEntityId, "OneLevelEntityId")
				.GeneratedBy.Guid()
				.UnsavedValue(Guid.Empty);

			Map(x => x.OneLevelEntityField, "OneLevelEntityField");

			References(x => x.RootEntityParent, "RootEntityId")
				.Nullable();
		}
	}
}
