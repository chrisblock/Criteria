using System;

using FluentNHibernate.Mapping;

namespace Criteria.NHibernate.IntegrationTests.TreeModel.Mappings
{
	public class ThreeLevelEntityGrandchildMapping : ClassMap<ThreeLevelEntityGrandchild>
	{
		public ThreeLevelEntityGrandchildMapping()
		{
			Table("ThreeLevelEntityGrandchild");

			Id(x => x.ThreeLevelEntityGrandchildId, "ThreeLevelEntityGrandchildId")
				.GeneratedBy.Guid()
				.UnsavedValue(Guid.Empty);

			Map(x => x.ThreeLevelEntityGrandchildField, "ThreeLevelEntityGrandchildField");

			References(x => x.ThreeLevelEntityChildParent, "ThreeLevelEntityChildId")
				.Nullable();
		}
	}
}
