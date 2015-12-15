// Useful C#
// Copyright (C) 2014-2016 Nicholas Randal
// 
// Useful C# is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

using System.IO.Fakes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;
using Rhino.Mocks;

namespace Randal.Tests.Logging
{
	[TestClass]
	public sealed class LoggerStringFormatDecoratorTests : BaseUnitTest<LoggerStringFormatDecoratorThens>
	{
		protected override void OnTeardown()
		{
			Then.Decorator.Dispose();
		}

		[TestMethod]
		public void ShouldHaveDecoratedLoggerWhenCreating()
		{
			When(Creating);
			Then.Decorator.Should().NotBeNull().And.BeAssignableTo<ILogger>();
			Then.Decorator.BaseLogger.Should().NotBeNull().And.BeAssignableTo<ILogger>();
		}

		[TestMethod]
		public void ShouldAddLogEntryToInternalLoggerWhenAddingLogEntry()
		{
			Given.Text = "Hey";
			When(AddingLogEntry);
			Then.Logger.AssertWasCalled(x => x.Add(Arg<LogEntry>.Is.NotNull));
		}

		[TestMethod]
		public void ShouldAddBlankLineWhenAddingBlank()
		{
			When(AddingBlank);
			Then.Logger.AssertWasCalled(x => x.Add(Arg<LogEntryNoTimestamp>.Is.NotNull));
		}

		[TestMethod]
		public void ShouldAddLogEntryToInternalLoggerWhenAddingLogEntryWithoutTimestamp()
		{
			Given.Text = "Hey again";
			When(AddingEntryWithoutTimestamp);
			Then.Logger.AssertWasCalled(x => x.Add(Arg<LogEntryNoTimestamp>.Is.NotNull));
		}

		[TestMethod]
		public void ShouldAddLogEntryToInternalLoggerWhenAddingException()
		{
			Given.Text = "Oops";
			Given.Exception = new StubFileLoadException();
			When(AddingException);
			Then.Logger.AssertWasCalled(x => x.Add(Arg<LogEntry>.Is.NotNull));
		}

		[TestMethod]
		public void ShouldCallInternalLoggerDisposeWhenDisposing()
		{
			When(Disposing);
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

		private void AddingLogEntry()
		{
			Then.Decorator.AddEntry(Given.Text);
		}

		private void AddingBlank()
		{
			Then.Decorator.AddBlank();
		}

		protected override void Creating()
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