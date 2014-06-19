using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Scripts;
using FluentAssertions;

namespace Randal.Tests.Utilities.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class CatalogBlockTests : BaseUnitTest<CatalogBlockThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveInvalidBlockWhenCreating()
		{
			Given.Text = " master ";

			When(Creating);

			Then.Object.Should().NotBeNull().And.BeAssignableTo<IScriptBlock>();
			Then.Object.IsValid.Should().BeFalse();
			Then.Object.Keyword.Should().Be("catalog");
			Then.Object.Text.Should().Be("master");
			Then.Object.CatalogPatterns.Should().HaveCount(0);
		}

		[TestMethod]
		public void ShouldHaveValidBlockWhenParsingGivenValidInput()
		{
			Given.Text = "master, DB123, X_%";

			When(Creating, Parsing);

			Then.Object.IsValid.Should().BeTrue();
			Then.Messages.Should().HaveCount(0);
			Then.Object.CatalogPatterns.Should().HaveCount(3);
			Then.Object.CatalogPatterns[0].Should().Be("^master$");
			Then.Object.CatalogPatterns[1].Should().Be("^DB123$");
			Then.Object.CatalogPatterns[2].Should().Be(@"^X_[_\w\d-]*$");
		}

		[TestMethod]
		public void ShouldHaveErrorMessagesWhenParsingGivenInvalidInput()
		{
			Given.Text = "t-d?, P&E, t;";

			When(Creating, Parsing);

			Then.Object.IsValid.Should().BeFalse();
			Then.Object.CatalogPatterns.Should().HaveCount(0);
			Then.Messages.Should().HaveCount(3);
		}

		[TestMethod]
		public void ShouldNotHaveErrorMessageWhenParsingUnqualifiedWildcard()
		{
			Given.Text = "%";

			When(Creating, Parsing);

			Then.Object.IsValid.Should().BeTrue();
			Then.Object.CatalogPatterns.Should().HaveCount(1);
			Then.Object.CatalogPatterns.First().Should().Be(@"^[_\w\d-]*$");
		}

		private void Parsing()
		{
			Then.Messages = Then.Object.Parse();
		}

		private void Creating()
		{
			Then.Object = new CatalogBlock(Given.Text);
		}
	}

	public sealed class CatalogBlockThens
	{
		public CatalogBlock Object;
		public IReadOnlyList<string> Messages;
	}
}