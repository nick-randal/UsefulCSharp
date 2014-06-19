using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Tests.Utilities.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class ConfigurationBlockTests : BaseUnitTest<TimeoutBlockThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveTimeoutBlockWhenCreaatingInstance()
		{
			Given.Json = "{}";

			When(Creating);

			Then.Configuration.Should().NotBeNull().And.BeAssignableTo<IScriptBlock>();
			Then.Configuration.Text.Should().Be("{}");
			Then.Configuration.IsValid.Should().BeFalse();
			Then.Configuration.Keyword.Should().Be("configuration");
			Then.Configuration.Settings.Should().BeNull();
		}

		[TestMethod]
		public void ShouldHaveValuesWhenParsingGivenValidJson()
		{
			Given.Json = "{ timeout: 256, usetransaction: false }";

			When(Creating, Parsing);

			Then.Settings.Should().NotBeNull();
			Then.Settings.Timeout.Should().Be(256);
			Then.Settings.UseTransaction.Should().BeFalse();
		}

		[TestMethod]
		public void ShouldHaveDefaultValuesWhenParsingGivenValidJsonWithoutValues()
		{
			Given.Json = "{ }";

			When(Creating, Parsing);

			Then.Settings.Should().NotBeNull();
			Then.Settings.Timeout.Should().Be(30);
			Then.Settings.UseTransaction.Should().BeTrue();
		}

		[TestMethod]
		public void ShouldHaveValuesWhenParsingGivenJsonWithoutEnclosingBraces()
		{
			Given.Json = "Timeout: 256, useTransaction: true";

			When(Creating, Parsing);

			Then.Settings.Should().NotBeNull();
			Then.Settings.Timeout.Should().Be(256);
			Then.Settings.UseTransaction.Should().BeTrue();
		}

		private void Creating()
		{
			Then.Configuration = new ConfigurationBlock(Given.Json);
		}

		private void Parsing()
		{
			Then.Messages = Then.Configuration.Parse();
			Then.Settings = Then.Configuration.Settings;
		}
	}

	public sealed class TimeoutBlockThens
	{
		public ConfigurationBlock Configuration;
		public IReadOnlyList<string> Messages;
		public ScriptSettings Settings;
	}
}