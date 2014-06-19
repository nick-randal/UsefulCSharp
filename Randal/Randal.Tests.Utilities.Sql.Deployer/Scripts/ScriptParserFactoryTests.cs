using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Tests.Utilities.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class ScriptParserFactoryTests : BaseUnitTest<ScriptParserFactoryThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveParserWithKnownRulesWhenCreatingStandardParser()
		{
			When(Creating);

			Then.Parser.RegisteredKeywords().Should().HaveCount(9);
			Then.Parser.RegisteredKeywords().Should()
				.Contain("catalog").And
				.Contain("options").And
				.Contain("config").And
				.Contain("configuration").And
				.Contain("need").And
				.Contain("ignore").And
				.Contain("pre").And
				.Contain("main").And
				.Contain("post");
			Then.Parser.HasFallbackRule.Should().BeTrue();
		}

		private void Creating()
		{
			Then.Factory = new ScriptParserFactory();
			Then.Parser = Then.Factory.CreateStandardParser();
		}
	}

	public sealed class ScriptParserFactoryThens
	{
		public ScriptParserFactory Factory;
		public IScriptParserConsumer Parser;
	}
}
