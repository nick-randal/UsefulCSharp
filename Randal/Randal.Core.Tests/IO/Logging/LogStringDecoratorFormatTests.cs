using System.IO.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.IO.Logging;
using Randal.Core.Testing.UnitTest;
using Rhino.Mocks;

namespace Randal.Tests.Core.IO.Logging
{
	[TestClass]
	public sealed class LogStringDecoratorFormatTests : BaseUnitTest<LoggerStringFormatDecoratorThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestCleanup]
		public void Teardown()
		{
			Then.Decorator.Dispose();
		}

		[TestMethod]
		public void ShouldHaveDecoratedLoggerWhenCreating()
		{
			When(Creating);
			Then.Decorator.Should().NotBeNull().And.BeAssignableTo<ILogger>();
		}

		[TestMethod]
		public void ShouldAddLogEntryToInternalLoggerWhenAddingLogEntry()
		{
			Given.Text = "Hey";
			When(Creating, AddingLogEntry);
			Then.Logger.AssertWasCalled(x => x.Add(Arg<LogEntry>.Is.NotNull));
		}

		[TestMethod]
		public void ShouldAddLogGroupEntryToInternalLoggerWhenAddingLogGroupEntry()
		{
			Given.Text = "My Group";
			When(Creating, AddingLogGroupEntry);
			Then.Logger.AssertWasCalled(x => x.Add(Arg<LogGroupEntry>.Is.NotNull));
		}

		[TestMethod]
		public void ShouldAddLogEntryToInternalLoggerWhenAddingLogEntryWithoutTimestamp()
		{
			Given.Text = "Hey again";
			When(Creating, AddingEntryWithoutTimestamp);
			Then.Logger.AssertWasCalled(x => x.Add(Arg<LogEntryNoTimestamp>.Is.NotNull));
		}

		[TestMethod]
		public void ShouldAddLogEntryToInternalLoggerWhenAddingException()
		{
			Given.Text = "Oops";
			Given.Exception = new StubFileLoadException();
			When(Creating, AddingException);
			Then.Logger.AssertWasCalled(x => x.Add(Arg<LogEntry>.Is.NotNull));
		}

		[TestMethod]
		public void ShouldCallInternalLoggerDisposeWhenDisposing()
		{
			When(Creating, Disposing);
			Then.Logger.AssertWasCalled(x => x.Dispose());
		}

		private void AddingEntryWithoutTimestamp()
		{
			Then.Decorator.AddEntryNoTimestamp(Given.Text);
		}

		private void AddingException()
		{
			Then.Decorator.AddException(Given.Exception, Given.Text);
		}

		private void AddingLogGroupEntry()
		{
			Then.Decorator.AddGroup(Given.Text);
		}

		private void AddingLogEntry()
		{
			Then.Decorator.AddEntry(Given.Text);
		}

		private void Creating()
		{
			Then.Logger = MockRepository.GenerateMock<ILogger>();
			Then.Decorator = new LoggerStringFormatDecorator(Then.Logger);
		}

		private void Disposing()
		{
			Then.Decorator.Dispose();
		}
	}

	public sealed class LoggerStringFormatDecoratorThens
	{
		public LoggerStringFormatDecorator Decorator;
		public ILogger Logger;
	}
}
