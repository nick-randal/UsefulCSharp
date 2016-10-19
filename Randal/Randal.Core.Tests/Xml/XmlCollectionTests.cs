// Useful C#
// Copyright (C) 2014-2016 Nicholas Randal
// 
// Useful C# is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using GwtUnit.UnitTest;
using Randal.Core.Xml;

namespace Randal.Tests.Core.Xml
{
	[TestClass]
	public sealed class XmlCollectionTests : UnitTestBase<XmlCollectionThens>
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
