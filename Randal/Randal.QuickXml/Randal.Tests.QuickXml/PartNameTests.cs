using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
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
