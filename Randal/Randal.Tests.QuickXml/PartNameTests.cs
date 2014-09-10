// // Useful C#
// // Copyright (C) 2014 Nicholas Randal
// // 
// // Useful C# is free software; you can redistribute it and/or modify
// // it under the terms of the GNU General Public License as published by
// // the Free Software Foundation; either version 2 of the License, or
// // (at your option) any later version.
// // 
// // This program is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// // GNU General Public License for more details.

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.QuickXml;

namespace Randal.Tests.QuickXml
{
	[TestClass]
	public sealed class PartNameTests : BaseUnitTest<PartNameThens>
	{
		[TestMethod]
		public void ShouldHaveValidPartName_WhenCreating_GivenOnePart()
		{
			Given.Name = "Assembly ";

			When(Creating);

			Then.Target.IsTwoPart.Should().BeFalse();
			Then.Target.One.Should().Be("Assembly");
		}

		[TestMethod]
		public void ShouldHaveValidPartName_WhenCreating_GivenTwoParts()
		{
			Given.Name = "xsd : Assembly";

			When(Creating);

			Then.Target.IsTwoPart.Should().BeTrue();
			Then.Target.One.Should().Be("xsd");
			Then.Target.Two.Should().Be("Assembly");
		}

		protected override void Creating()
		{
			Then.Target = new PartName(Given.Name);
		}
	}

	public sealed class PartNameThens
	{
		public PartName Target;
	}
}