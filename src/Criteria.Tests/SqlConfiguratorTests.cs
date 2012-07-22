// ReSharper disable InconsistentNaming

using Criteria.Sql;
using Criteria.Tests.TestModel;

using NUnit.Framework;

namespace Criteria.Tests
{
	[TestFixture]
	public class SqlConfiguratorTests
	{
		[Test]
		public void MakeDistinct_SetsDistinctToTrue()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.MakeDistinct();

			Assert.That(configuration.Distinct, Is.True);
			Assert.That(configuration.DistinctCountPropertyExpression, Is.Null);
		}

		[Test]
		public void MakeDistinct_GetCount_Throws()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.GetCount();

			Assert.That(() => configurator.MakeDistinct(), Throws.InvalidOperationException);
		}

		[Test]
		public void MakeDistinct_StarSelect_Throws()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.StarSelect();

			Assert.That(() => configurator.MakeDistinct(), Throws.InvalidOperationException);
		}

		[Test]
		public void GetCount_SetsCountToTrue()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.GetCount();

			Assert.That(configuration.Count, Is.True);
			Assert.That(configuration.DistinctCountPropertyExpression, Is.Null);
		}

		[Test]
		public void GetCount_MakeDistinct_Throws()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.MakeDistinct();

			Assert.That(() => configurator.GetCount(), Throws.InvalidOperationException);
		}

		[Test]
		public void GetCount_Unalias_Throws()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.Unalias();

			Assert.That(() => configurator.GetCount(), Throws.InvalidOperationException);
		}

		[Test]
		public void GetCount_StarSelect_Throws()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.StarSelect();

			Assert.That(() => configurator.GetCount(), Throws.InvalidOperationException);
		}

		[Test]
		public void StarSelect_SetsStarSelectToTrue()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.StarSelect();

			Assert.That(configuration.StarSelect, Is.True);
			Assert.That(configuration.DistinctCountPropertyExpression, Is.Null);
		}

		[Test]
		public void StarSelect_MakeDistinct_Throws()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.MakeDistinct();

			Assert.That(() => configurator.StarSelect(), Throws.InvalidOperationException);
		}

		[Test]
		public void StarSelect_Unalias_Throws()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.Unalias();

			Assert.That(() => configurator.StarSelect(), Throws.InvalidOperationException);
		}

		[Test]
		public void StarSelect_GetCount_Throws()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.GetCount();

			Assert.That(() => configurator.StarSelect(), Throws.InvalidOperationException);
		}

		[Test]
		public void Unalias_SetsUnaliasToTrue()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.Unalias();

			Assert.That(configuration.Unalias, Is.True);
			Assert.That(configuration.DistinctCountPropertyExpression, Is.Null);
		}

		[Test]
		public void Unalias_StarSelect_Throws()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.StarSelect();

			Assert.That(() => configurator.Unalias(), Throws.InvalidOperationException);
		}

		[Test]
		public void Unalias_GetCount_Throws()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.GetCount();

			Assert.That(() => configurator.Unalias(), Throws.InvalidOperationException);
		}

		[Test]
		public void CountDistinctProperty_SetsCountDistinctProperty()
		{
			var configuration = new SqlGeneratorConfiguration();
			var configurator = new SqlGeneratorConfigurator(configuration);

			configurator.CountDistinctProperty((TestEntityClass e) => e.TestIntegerProperty);

			Assert.That(configuration.DistinctCountPropertyExpression, Is.Not.Null);
		}
	}
}
