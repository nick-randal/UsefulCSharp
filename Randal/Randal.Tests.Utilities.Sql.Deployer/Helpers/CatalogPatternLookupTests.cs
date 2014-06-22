using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Helpers;

namespace Randal.Tests.Utilities.Sql.Deployer.Helpers
{
	[TestClass]
	public sealed class CatalogPatternLookupTests : BaseUnitTest<CatalogPatternLookupThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveEmptyLookupWhenCreatingInstance()
		{
			When(Creating);

			Then.Lookup.Should().NotBeNull();
			Then.Lookup.Count.Should().Be(0);
		}

		[TestMethod]
		public void ShouldHaveRegexWhenGettingPatternGivenText()
		{
			Given.Text = "ma%er";

			When(Creating, Getting);

			Then.Pattern.Options.Should().Be(RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
			Then.Lookup.Count.Should().Be(1);
			Then.Pattern.ToString().Should().Be(@"^ma[._\w\d-]*er$");
		}

		private void Getting()
		{
			Then.Pattern = Then.Lookup[Given.Text];
		}

		private void Creating()
		{
			Then.Lookup = new CatalogPatternLookup();
		}
	}

	public sealed class CatalogPatternLookupThens
	{
		public CatalogPatternLookup Lookup;
		public Regex Pattern;
	}
}
