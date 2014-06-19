using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Tests.Utilities.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class UnexpectedBlockTests : BaseUnitTest<UnexpectedBlockThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
			Given.Keyword = "unknown";
		}

		[TestMethod]
		public void ShouldCreateValidInstanceWhenCreatingUnexpectedBlockFromText()
		{
			Given.Text = "--:: unknown\nselect 1\nGO\n";

			When(CreatingInstace);

			Then.Object.Should().NotBeNull().And.BeAssignableTo<IScriptBlock>();
			Then.Object.IsValid.Should().BeFalse();
			Then.Object.Keyword.Should().Be("unknown");
			Then.Object.Text.Should().Be("--:: unknown\nselect 1\nGO");
		}

		[TestMethod]
		public void ShouldLeaveTextUnalteredAndHaveErrorMessageWhenParsingAnUnexpectedBlock()
		{
			Given.Text = "--:: unknown\nselect 1\nGO\n";

			When(CreatingInstace, Parsing);

			Then.Messages.Should().HaveCount(1);
			Then.Messages.First().Should().Be("Unexpected keyword 'unknown' found for this block.");
		}
		
		private void CreatingInstace()
		{
			Then.Object = new UnexpectedBlock(Given.Keyword, Given.Text);
		}

		private void Parsing()
		{
			Then.Messages = Then.Object.Parse();
		}
	}

	public sealed class UnexpectedBlockThens
	{
		public UnexpectedBlock Object;
		public IReadOnlyList<string> Messages;
	}
}
