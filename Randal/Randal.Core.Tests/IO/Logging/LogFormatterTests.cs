/*
Useful C#
Copyright (C) 2014  Nicholas Randal

Useful C# is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.IO.Logging;
using Randal.Core.Testing.UnitTest;

namespace Randal.Core.Core.IO.Logging
{
	[TestClass]
	public sealed class LogFormatterTests : BaseUnitTest<LogFormatterThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();

			Given.EntryCount = 1;
			Given.Inset = null;
		}

		[TestMethod]
		public void ShouldHaveValidLogFormatterWhenCreating()
		{
			When(Creating);

			Then.Formatter.Should().NotBeNull()
				.And.BeAssignableTo<ILogEntryFormatter>();
		}

		[TestMethod]
		public void ShouldHaveValidTextWhenFormattingEntryGivenValidLogEntry()
		{
			Given.Entry = new LogEntry("Hello", new DateTime(2014, 1, 31, 7, 0, 0));
			When(Creating, FormattingEntry);
			Then.Text.Should().Be("140131 070000 Hello\r\n");
		}

		[TestMethod]
		public void ShouldHaveValidTextWhenFormattingEntryGivenValidLogEntryNoTimestamp()
		{
			Given.Entry = new LogEntryNoTimestamp("Hello");
			When(Creating, FormattingEntry);
			Then.Text.Should().Be(TextResources.NoTimestamp + ' ' + "Hello\r\n");
		}


		private void FormattingEntry()
		{
			for(var n = 0; n < Given.EntryCount; n++)
				Then.Text = Then.Formatter.Format(Given.Entry);
		}

		private void Creating()
		{
			Then.Formatter = new LogEntryFormatter();
		}
	}

	public sealed class LogFormatterThens
	{
		public LogEntryFormatter Formatter;
		public string Text;
	}
}
