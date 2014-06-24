// Useful C#
// Copyright (C) 2014 Nicholas Randal
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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			Then.Entry = Given.TestForMember("Verbosity")
				? new LogEntryNoTimestamp(Given.Message, Given.Verbosity)
				: new LogEntryNoTimestamp(Given.Message);
		}
	}

	public sealed class LogEntryNoTimestampThens
	{
		public LogEntryNoTimestamp Entry;
	}
}