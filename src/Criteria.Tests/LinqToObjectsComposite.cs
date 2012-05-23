using Criteria.Tests.LinqToObjectsModel;

namespace Criteria.Tests
{
	public class LinqToObjectsComposite
	{
		public LinqToObjectsOne One { get; set; }
		public LinqToObjectsTwo Two { get; set; }

		public LinqToObjectsComposite(LinqToObjectsOne one, LinqToObjectsTwo two)
		{
			One = one;
			Two = two;
		}
	}
}
