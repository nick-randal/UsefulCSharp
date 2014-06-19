using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Tests.Utilities.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class IgnoreScriptBlockTests : BaseUnitTest<IgnoreScriptBlockThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveValidIgnoreScriptBlockWhenCreating()
		{
			Given.Text = "use TCPLP\nGO\n";

			When(Creating);

			Then.Object.Should().NotBeNull().And.BeAssignableTo<BaseScriptBlock>();
			Then.Object.IsValid.Should().BeTrue();
			Then.Object.Text.Should().Be("use TCPLP\nGO");
		}

		private void Creating()
		{
			Then.Object = new IgnoreScriptBlock(Given.Text);
		}
	}

	public sealed class IgnoreScriptBlockThens
	{
		public IgnoreScriptBlock Object;
	}
}