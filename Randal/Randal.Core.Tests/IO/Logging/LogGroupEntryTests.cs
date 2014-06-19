using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.IO.Logging;
using Randal.Core.Testing.UnitTest;
using Rhino.Mocks;

namespace Randal.Tests.Core.IO.Logging
{
	[TestClass]
	public sealed class LogGroupEntryTests : BaseUnitTest<LogGroupEntryThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveValidLogGroupEntryWhenCreating()
		{
			Given.Logger = null;
			Given.Name = "Group";
			Given.Verbosity = Verbosity.Important;
			
			When(Creating);

			Then.Entry.Should().NotBeNull()
				.And.BeAssignableTo<ILogEntry>()
				.And.BeAssignableTo<ILogGroupEntry>();
			Then.Entry.Message.Should().Be("Group");
			Then.Entry.ShowTimestamp.Should().BeFalse();
			Then.Entry.Timestamp.Should().Be(DateTime.MinValue);
			Then.Entry.IsEnd.Should().BeFalse();
			Then.Entry.VerbosityLevel.Should().Be(Verbosity.Important);
		}

		[TestMethod]
		public void ShouldAddGroupEntryMarkedAsIsEndWhenDisposingGivenLoggerInstance()
		{
			Given.Logger = MockRepository.GenerateMock<ILogger>();

			Given.Name = "GroupA";
			Given.Verbosity = Verbosity.Info;

			When(Creating, Disposing);

			Then.Logger.AssertWasCalled(x => x.Add(Arg<ILogGroupEntry>.Is.NotNull));
		}

		private void Creating()
		{
			Then.Entry = new LogGroupEntry(Given.Logger, Given.Name, Given.Verbosity);
		}

		private void Disposing()
		{
			Then.Logger = Given.Logger;
			Then.Entry.Dispose();
		}
	}

	public sealed class LogGroupEntryThens
	{
		public LogGroupEntry Entry;
		public ILogger Logger;
	}
}
