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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;

namespace Randal.Tests.Logging
{
	[TestClass]
	public sealed class LogEntryNoTimestampTests : UnitTestBase<LogEntryNoTimestampThens>
	{
		[TestMethod]
		public void ShouldHaveValidLogEntryWhenCreating()
		{
			When(Creating);

			Then.Entry.Should().NotBeNull().And.BeAssignableTo<ILogEntry>();
			Then.Entry.ShowTimestamp.Should().BeFalse();
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

		protected override void Creating()
		{
			Then.Entry = GivensDefined("Verbosity")
				? new LogEntryNoTimestamp(Given.Message, Given.Verbosity)
				: new LogEntryNoTimestamp(Given.Message);
		}
	}

	public sealed class LogEntryNoTimestampThens
	{
		public LogEntryNoTimestamp Entry;
	}
}