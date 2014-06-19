using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Tests.Utilities.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class NeedBlockTests : BaseUnitTest<NeedBlockThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
			Given.Text = string.Empty;
		}

		[TestMethod]
		public void ShouldHaveInvalidBlockWhenCreating()
		{
			When(Creating);

			Then.Object.Should().BeAssignableTo<BaseScriptBlock>();
			Then.Object.IsValid.Should().BeFalse();
			Then.Object.Keyword.Should().Be("need");
		}

		[TestMethod]
		public void ShouldHaveValidBlockWhenParsingGivenValidInput()
		{
			Given.Text = "A, B, C.sql,,D";

			When(Creating, WhenParsing);

			Then.Object.IsValid.Should().BeTrue();
			Then.Object.Files.Should().HaveCount(4);
			Then.Object.Files[0].Should().Be("A.sql");
			Then.Object.Files[1].Should().Be("B.sql");
			Then.Object.Files[2].Should().Be("C.sql");
			Then.Object.Files[3].Should().Be("D.sql");
		}

		[TestMethod]
		public void ShouldBeInvalidWhenParsingGivenListOfInvalidFileNames()
		{
			Given.Text = "File*A, File?B, Procedures\\ReadAll";

			When(Creating, WhenParsing);

			Then.Object.IsValid.Should().BeFalse();
			Then.Messages.Should().HaveCount(3);
		}

		private void WhenParsing()
		{
			Then.Messages = Then.Object.Parse();
		}

		private void Creating()
		{
			Then.Object = new NeedBlock(Given.Text);
		}
	}

	public sealed class NeedBlockThens
	{
		public NeedBlock Object;
		public IReadOnlyList<string> Messages;
	}
}
