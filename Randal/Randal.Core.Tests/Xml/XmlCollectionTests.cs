using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Core.Xml;

namespace Randal.Tests.Core.Xml
{
	[TestClass]
	public sealed class XmlCollectionTests : BaseUnitTest<XmlCollectionThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveCollection_WhenCreatingInstance_GivenNoItems()
		{
			When(Creating);

			Then.Collection.Should().NotBeNull();
			Then.Collection.Items.Should().HaveCount(0);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveCollection_WhenCreatingInstance_GivenItems()
		{
			Given.Items = new[] { 1, 3, 5, 7 };
			When(Creating);

			Then.Collection.Should().NotBeNull();
			Then.Collection.Items
				.Should().HaveCount(4)
				.And.Equal(new [] { 1, 3, 5, 7 });
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveOneItem_WhenAddingItem_GivenValue()
		{
			Given.ItemToAdd = 47;

			When(AddingItem);

			Then.Collection.Items.Should().HaveCount(1).And.Contain(47);
		}

		protected override void Creating()
		{
			Then.Collection = GivensDefined("Items") ? new XmlCollection<int>(Given.Items) : new XmlCollection<int>();
		}

		private void AddingItem()
		{
			Then.Collection.Add(Given.ItemToAdd);
		}
	}

	public sealed class XmlCollectionThens
	{
		public XmlCollection<int> Collection;
	}
}
