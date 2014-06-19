using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Core.IO.Logging;

namespace Randal.Tests.Core.IO.Logging
{
	[TestClass]
	public sealed class AsyncFileLoggerTests : BaseUnitTest<AsyncFileLoggerThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();

			Given.NullSettings = false;
		}

		[TestMethod]
		public void ShouldHaveFileLoggerWhenCreating()
		{
			When(Creating);

			Then.Logger.Should().NotBeNull().And.BeAssignableTo<ILogger>();
			Then.Logger.VerbosityThreshold.Should().Be(Verbosity.All);
			Then.Logger.State.Should().Be(AsyncFileLoggerState.Running);
		}

		[TestMethod]
		public void ShouldChangeValueWhenSettingVerbosityGivenNewVerbosityLevel()
		{
			Given.Verbosity = Verbosity.Important;

			When(Creating, SettingVerbosity);

			Then.Logger.VerbosityThreshold.Should().Be(Verbosity.Important);
		}

		[TestMethod]
		public void ShouldBeDisposedWhenDisposingLogger()
		{
			When(Creating, Disposing);
			Then.Logger.State.Should().Be(AsyncFileLoggerState.Disposed);
		}

		[TestMethod, ExpectedException(typeof (ArgumentNullException))]
		public void ShouldThrowExceptionWhenCreatingGivenNullSettings()
		{
			Given.NullSettings = true;

			When(Creating);
		}

		private void Disposing()
		{
			Then.Logger.Dispose();
		}

		private void SettingVerbosity()
		{
			Then.Logger.ChangeVerbosityThreshold(Given.Verbosity);
		}

		private void Creating()
		{
			var settings = new FileLoggerSettings(Test.Paths.IoLoggingFolder, "Test", 1024);
			Then.Logger = new AsyncFileLogger(Given.NullSettings ? null : settings);
			Thread.Sleep(100);
		}
	}

	public sealed class AsyncFileLoggerThens
	{
		public AsyncFileLogger Logger;
	}
}
