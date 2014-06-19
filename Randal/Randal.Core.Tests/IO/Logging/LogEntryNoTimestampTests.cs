using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.IO.Logging;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.IO.Logging
{
	[TestClass]
	public sealed class LogEntryNoTimestampTests : BaseUnitTest<LogEntryNoTimestampThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveValidLogEntryWhenCreating()
		{
			When(Creating);
			Then.Entry.Should().NotBeNull().And.BeAssignableTo<ILogEntry>();
			Then.Entry.ShowTimestamp.Should().BeFalse();
			Then.Entry.Timestamp.Should().Be(DateTime.MinValue);
		}

		[TestMethod]
		public void ShouldHaveValidLogEntryWhenCreatingGivenValues()
		{
			Given.Message = "Hello";
			Given.Verbosity = Verbosity.Vital;

			When(Creating);

			Then.Entry.Should().NotBeNull().And.BeAssignableTo<ILogEntry>();
			Then.Entry.Message.Should().Be("Hello");
			Then.Entry.VerbosityLevel.Should().Be(Verbosity.Vital);
		}

		private void Creating()
		{
			Then.Entry = Given.TestForMember("Verbosity") ? 
				new LogEntryNoTimestamp(Given.Message, Given.Verbosity) :
				new LogEntryNoTimestamp(Given.Message);
		}
	}

	public sealed class LogEntryNoTimestampThens
	{
		public LogEntryNoTimestamp Entry;
	}
}
