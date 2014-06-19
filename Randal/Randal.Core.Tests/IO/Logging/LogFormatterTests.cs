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

		[TestMethod]
		public void ShouldHaveValidTextWhenFormattingEntryGivenValidLogGroupEntry()
		{
			Given.Entry = new LogGroupEntry(null, "Hello");
			When(Creating, FormattingEntry);
			Then.Text.Should().Be(TextResources.NoTimestamp + ' ' + TextResources.GroupLeadIn + "Hello\r\n");
		}

		[TestMethod]
		public void ShouldHaveValidTextWithInsetWhenFormattingGivenMultipleLogGroupEntries()
		{
			Given.Inset = "*";
			Given.EntryCount = 6;
			Given.Entry = new LogGroupEntry(null, "Hello");

			When(Creating, FormattingEntry);

			Then.Text.Should().Be(TextResources.NoTimestamp + ' ' + "*****" + TextResources.GroupLeadIn + "Hello\r\n");
		}

		private void FormattingEntry()
		{
			for(var n = 0; n < Given.EntryCount; n++)
				Then.Text = Then.Formatter.Format(Given.Entry);
		}

		private void Creating()
		{
			Then.Formatter = new LogEntryFormatter(Given.Inset);
		}
	}

	public sealed class LogFormatterThens
	{
		public LogEntryFormatter Formatter;
		public string Text;
	}
}
