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
using System.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.IO.Logging;
using Randal.Core.Testing.UnitTest;
using System.Net.Mail;
using System.Data.SqlClient;

namespace Randal.Tests.Core.IO.Logging
{
	[TestClass]
	public sealed class ExceptionEntryTests : BaseUnitTest<UnitTest2Thens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();

			Given.SystemDateTime = new DateTime();
		}

		[TestMethod]
		public void ShouldHaveValidEntryWhenCreating()
		{
			Given.Exception = new InvalidTimeZoneException();
			Given.SystemDateTime = new DateTime(2014, 6, 13, 1, 2, 3);

			WhenCreating();

			Then.Entry.Should().NotBeNull().And.BeAssignableTo<ILogEntry>();
			Then.Entry.VerbosityLevel.Should().Be(Verbosity.Vital);
			Then.Entry.ShowTimestamp.Should().BeTrue();
			Then.Entry.Timestamp.Should().Be(new DateTime(2014, 6, 13, 1, 2, 3));
			Then.Entry.Message.Should().Be(
				"Error Info\r\n--------------------------------------------------------------------------------\r\n" +
				"System.InvalidTimeZoneException : Exception of type 'System.InvalidTimeZoneException' was thrown.\r\n\r\n" +
				"Stack Trace\r\n\r\n\r\n--------------------------------------------------------------------------------\r\n"
				);
		}

		[TestMethod]
		public void ShouldHaveNoExceptionMessageWhenCreatingGivenNullExcpetion()
		{
			Given.Exception = null;

			WhenCreating();

			Then.Entry.Message.Should().Be("An error occurred but no instance of Exception provided.");
		}

		[TestMethod]
		public void ShouldHaveSpecialFormatWhenCreatingGivenSmtpException()
		{
			Given.Exception = new SmtpException(SmtpStatusCode.ClientNotPermitted);

			WhenCreating();

			Then.Entry.Message.Should().Be(
				"Error Info\r\n--------------------------------------------------------------------------------\r\n" +
				"SMTP Error : ClientNotPermitted - Client does not have permission to submit mail to this server.\r\n\r\n" +
				"Stack Trace\r\n\r\n\r\n--------------------------------------------------------------------------------\r\n");
		}

		[TestMethod]
		public void ShouldHaveCustomMessageWhenCreatingGivenMessageText()
		{
			Given.Exception = new ArgumentException();
			Given.Message = "Hello error!";

			WhenCreating();

			Then.Entry.Message.Should().Be(
				"Hello error!\r\nError Info\r\n--------------------------------------------------------------------------------\r\n"+
				"System.ArgumentException : Value does not fall within the expected range.\r\n\r\n"+
				"Stack Trace\r\n\r\n\r\n--------------------------------------------------------------------------------\r\n"
				);
		}

		[TestMethod]
		public void ShouldHaveSpecialFormatWhenCreatingGivenSqlException()
		{
			Given.Exception = CreateSqlException();

			WhenCreating();

			Then.Entry.Message.Should().Contain("SqlException");
		}

		private static SqlException CreateSqlException()
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

		private void WhenCreating()
		{
			using (ShimsContext.Create())
			{
				ShimDateTime.NowGet = () => Given.SystemDateTime;

				if (Given.TestForMember("Message"))
					Then.Entry = new ExceptionEntry(Given.Exception, Given.Message);
				else
					Then.Entry = new ExceptionEntry(Given.Exception);
			}
		}
	}

	public sealed class UnitTest2Thens
	{
		public ExceptionEntry Entry;
	}
}