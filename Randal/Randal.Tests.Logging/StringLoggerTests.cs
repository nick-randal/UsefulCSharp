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

using System;
using System.Fakes;
using FluentAssertions;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;

namespace Randal.Tests.Logging
{
	[TestClass]
	public sealed class StringLoggerTests : UnitTestBase<StringLoggerThens>
	{
		[TestMethod]
		public void ShouldHaveValidLoggerWhenCreating()
		{
			When(Creating);

			Then.Logger.Should().NotBeNull().And.BeAssignableTo<ILogSink>();
			Then.Logger.VerbosityThreshold.Should().Be(Verbosity.All);
		}

		[TestMethod]
		public void ShouldHaveSameTextWhenGettingTextGivenValue()
		{
			Given.Entry = Entry("Hello world");

			When(AddingEntry, GettingText);

			Then.Text.Should().Be("151216 000000    Hello world\r\n");
		}

		[TestMethod]
		public void ShouldChangeValueWhenChangingVerbosityGivenDifferentVerbosityLevel()
		{
			Given.Verbosity = Verbosity.Vital;

			When(GettingChangedVerbosity);

			Then.Verbosity.Should().Be(Verbosity.Vital);
		}

		[TestMethod]
		public void ShouldHaveLoggedTextAvailableWhenGettingTextGivenDisposedLogger()
		{
			Given.Entry = Entry("Nothing is ever truly lost");

			When(AddingEntry, Disposing, GettingText);

			Then.Text.Should().Be("151216 000000    Nothing is ever truly lost\r\n");
		}

		protected override void Creating()
		{
			Then.Logger = new StringLogSink();
		}

		private void Disposing()
		{
			Then.Logger.Dispose();
		}

		private void GettingChangedVerbosity()
		{
			Then.Logger.ChangeVerbosityThreshold(Given.Verbosity);
			Then.Verbosity = Then.Logger.VerbosityThreshold;
		}

		private void AddingEntry()
		{
			Then.Logger.Post(Given.Entry);
		}

		private void GettingText()
		{
			Then.Text = Then.Logger.GetLatestText();
		}

		private static LogEntry Entry(string message)
		{
			using (ShimsContext.Create())
			{
				ShimDateTime.NowGet = () => new DateTime(2015, 12, 16, 0, 0, 0);
				return new LogEntry(message);
			}
		}
	}

	public sealed class StringLoggerThens : IDisposable
	{
		public StringLogSink Logger;
		public string Text;
		public Verbosity Verbosity;

		public void Dispose()
		{
			if (Logger != null)
				Logger.Dispose();
		}
	}
}