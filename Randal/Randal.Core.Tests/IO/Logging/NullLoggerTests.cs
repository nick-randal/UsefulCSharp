using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.IO.Logging;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.IO.Logging
{
	[TestClass]
	public sealed class NullLoggerTests : BaseUnitTest<NullLoggerThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestCleanup]
		public void Teardown()
		{
			if(Then.Log != null)
				Then.Log.Dispose();
		}

		[TestMethod]
		public void ShouldHaveValidNullLoggerWhenCreating()
		{
			When(Creating);

			Then.Log.Should().NotBeNull().And.BeAssignableTo<ILogger>();
			Then.Log.VerbosityThreshold.Should().Be(Verbosity.All);
		}

		[TestMethod]
		public void ShouldNotThrowExceptionWhenAddingAnEntry()
		{
			Given.Entry = new LogEntry("Test");

			When(Creating, AddingLogEntry);
		}

		[TestMethod]
		public void ShouldHaveUnchangedVerbosityLevelWhenChangingVerbosity()
		{
			Given.Verbosity = Verbosity.Vital;
			When(Creating, ChangingVerbosity);
			Then.Log.VerbosityThreshold.Should().Be(Verbosity.All);
		}

		private void ChangingVerbosity()
		{
			Then.Log.ChangeVerbosityThreshold(Given.Verbosity);
		}

		private void AddingLogEntry()
		{
			Then.Log.Add(Given.Entry);
		}

		private void Creating()
		{
			Then.Log = new NullLogger();
		}
	}

	public sealed class NullLoggerThens
	{
		public NullLogger Log;
	}
}
