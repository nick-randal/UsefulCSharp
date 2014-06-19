using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Tests.Utilities.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class ScriptSettingsTests : BaseUnitTest<ScriptSettingsThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveDefaultValuesWhenCreating()
		{
			When(Creating);

			Then.Settings.Timeout.Should().Be(30);
			Then.Settings.UseTransaction.Should().BeTrue();
		}

		[TestMethod]
		public void ShouldHaveValuesWhenCreatingGivenValues()
		{
			Given.Timeout = 357;
			Given.UseTransaction = false;

			When(Creating);

			Then.Settings.Timeout.Should().Be(357);
			Then.Settings.UseTransaction.Should().BeFalse();
		}

		private void Creating()
		{
			if(Given.TestForMember("Timeout") && Given.TestForMember("UseTransaction"))
				Then.Settings = new ScriptSettings(Given.Timeout, Given.UseTransaction);
			else
				Then.Settings = new ScriptSettings();
		}
	}

	public sealed class ScriptSettingsThens
	{
		public ScriptSettings Settings;
	}
}
