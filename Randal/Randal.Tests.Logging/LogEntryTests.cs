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
	public sealed class LogEntryTests : UnitTestBase<LogEntryThens>
	{
		[TestMethod]
		public void ShouldHaveValidLogEntryWhenCreatingGivenDefaults()
		{
			Given.Message = null;
			Given.Timestamp = new DateTime(2014, 6, 9);

			When(Creating);

			Then.Entry.Should().NotBeNull().And.BeAssignableTo<ILogEntry>();
			Then.Entry.Message.Should().Be(string.Empty);
			Then.Entry.Timestamp.Should().Be(new DateTime(2014, 6, 9));
			Then.Entry.VerbosityLevel.Should().Be(Verbosity.Info);
		}

		[TestMethod]
		public void ShouldHaveValigLogEntryWhenCreatingGivenValues()
		{
			Given.Message = "Hello";
			Given.Timestamp = new DateTime(2014, 1, 31);
			Given.Verbosity = Verbosity.Important;

			When(Creating);

			Then.Entry.Message.Should().Be("Hello");
			Then.Entry.Timestamp.Should().Be(new DateTime(2014, 1, 31));
			Then.Entry.VerbosityLevel.Should().Be(Verbosity.Important);
		}

		protected override void Creating()
		{
			using (ShimsContext.Create())
			{
				ShimDateTime.NowGet = () => Given.Timestamp;
				Then.Entry = GivensDefined("Verbosity") ? new LogEntry(Given.Message, Given.Verbosity) : new LogEntry(Given.Message);
			}
		}
	}

	public sealed class LogEntryThens
	{
		public LogEntry Entry;
		public LogEntry ClonedEntry;
	}
}