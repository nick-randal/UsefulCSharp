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
using System.Data.SqlClient;
using System.Fakes;
using System.Net.Mail;
using FluentAssertions;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;

namespace Randal.Tests.Logging
{
	[TestClass]
	public sealed class LogFormatterTests : UnitTestBase<LogFormatterTests.Thens>
	{
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
			Given.Entry = Entry("Hello");
			When(Formatting);
			Then.Text.Should().Be("151216 000000    Hello\r\n");
		}

		[TestMethod]
		public void ShouldHaveValidTextWhenFormattingEntryGivenValidLogEntryNoTimestamp()
		{
			Given.Entry = new LogEntryNoTimestamp("Hello");
			When(Formatting);
			Then.Text.Should().Be(TextResources.NoTimestamp + "    " + "Hello\r\n");
		}

		[TestMethod]
		public void ShouldHaveNoExceptionMessageWhenCreatingGivenNullExcpetion()
		{
			Given.Entry = new LogExceptionEntry(null);

			When(Formatting);

			Then.Text.Should().Be("An error occurred but no instance of Exception provided.\r\n");
		}

		[TestMethod]
		public void ShouldHaveSpecialFormatWhenCreatingGivenSmtpException()
		{
			Given.Entry = new LogExceptionEntry(new SmtpException(SmtpStatusCode.ClientNotPermitted));

			When(Formatting);

			Then.Text.Should().Be(
				"Error Info\r\n--------------------------------------------------------------------------------\r\n" +
				"SMTP Error : ClientNotPermitted - Client does not have permission to submit mail to this server.\r\n\r\n" +
				"Stack Trace\r\n\r\n\r\n--------------------------------------------------------------------------------\r\n"
				);
		}

		[TestMethod]
		public void ShouldHaveCustomMessageWhenCreatingGivenMessageText()
		{
			Given.Entry = new LogExceptionEntry(new ArgumentException(), "Hello error!");

			When(Formatting);

			Then.Text.Should().Be(
				"Hello error!\r\nError Info\r\n--------------------------------------------------------------------------------\r\n" +
				"System.ArgumentException : Value does not fall within the expected range.\r\n\r\n" +
				"Stack Trace\r\n\r\n\r\n--------------------------------------------------------------------------------\r\n"
				);
		}

		[TestMethod]
		public void ShouldHaveSpecialFormatWhenCreatingGivenSqlException()
		{
			Given.Entry = new LogExceptionEntry(SqlException());

			When(Formatting);

			Then.Text.Should().Contain("SqlException");
		}

		private static LogEntry Entry(string message)
		{
			using (ShimsContext.Create())
			{
				ShimDateTime.NowGet = () => new DateTime(2015, 12, 16, 0, 0, 0);
				return new LogEntry(message);
			}
		}

		private static SqlException SqlException()
		{
			try
			{
				var conn = new SqlConnection(@"Data Source=.;Database=GUARANTEED_TO_FAIL");
				conn.Open();
			}
			catch (SqlException ex)
			{
				return ex;
			}
			return null;
		}

		private void Formatting()
		{
			Then.Text = Then.Formatter.Format(Given.Entry);
		}

		protected override void Creating()
		{
			Then.Formatter = new LogEntryFormatter();
		}

		public sealed class Thens
		{
			public LogEntryFormatter Formatter;
			public string Text;
		}
	}
}