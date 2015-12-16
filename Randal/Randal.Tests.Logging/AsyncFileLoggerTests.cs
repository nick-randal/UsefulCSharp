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
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;
using Rhino.Mocks;

namespace Randal.Tests.Logging
{
	[TestClass]
	public sealed class AsyncFileLoggerTests : UnitTestBase<AsyncFileLoggerThens>
	{
		[TestMethod]
		public void ShouldHaveFileLogger_WhenCreating()
		{
			When(Creating);

			Then.Logger.Should().NotBeNull().And.BeAssignableTo<ILogSink>();
			Then.Logger.VerbosityThreshold.Should().Be(Verbosity.All);
		}

		[TestMethod]
		public void ShouldThrowException_WhenCreating_GivenNullSettings()
		{
			Given.NullSettings = true;

			WhenLastActionDeferred(Creating);

			ThenLastAction.ShouldThrow<ArgumentNullException>();
		}

		[TestMethod]
		public void ShouldHaveText_WhenLogging_GivenEntries()
		{
			Given.Entries = new[] { CreateEntry("Yay for logging.") };

			When(Logging, Disposing);

			Then.Text.Should().Be("151216 000000    Yay for logging.\r\n");
		}

		[TestMethod]
		public void ShouldNotHaveText_WhenLogging_GivenLowerVerbosityThanThreshold()
		{
			Given.Verbosity = Verbosity.Important;
			Given.Entries = new[] { new LogEntry("Just informational.") };

			When(Logging, Disposing);

			Then.Text.Should().Be("");
		}

		[TestMethod]
		public void ShouldHaveText_WhenLogging_GivenEqualVerbosityToThreshold()
		{
			Given.Verbosity = Verbosity.Important;
			Given.Entries = new[] { CreateEntry("This is important.", Verbosity.Important) };

			When(Logging, Disposing);

			Then.Text.Should().Be("151216 000000    This is important.\r\n");
		}

		[TestMethod]
		public void ShouldHaveTruncatedText_WhenLogging_GivenDuplicateEntries()
		{
			Given.Entries = new[]
			{
				new LogEntryNoTimestamp("Can you hear me now?"),
				new LogEntryNoTimestamp("Can you hear me now?"),
				new LogEntryNoTimestamp("Good")
			};

			When(Logging, Disposing);

			Then.Text.Should()
				.Be(
					"                     Can you hear me now?\r\nATTENTION: The previous line was repeated 2 times.\r\n                     Good\r\n");
		}

		[TestMethod]
		public void ShouldHaveTruncatedText_WhenLogging_GivenDuplicateEntries2()
		{
			Given.AllowRepeats = false;
			Given.Entries = Enumerable.Repeat(new LogEntryNoTimestamp(new string('X', 1000)), 100).ToArray();

			When(Logging, Disposing);

			Then.Text.Length.Should().Be(102300);
		}

		protected override void Creating()
		{
			var settings = new RollingFileSettings(Test.Paths.LoggingFolder, "Test", 1024, 
				Given.AllowRepeats == null || Given.AllowRepeats, null);

			if (GivensDefined("NullSettings") && Given.NullSettings == true)
				Then.Logger = new RollingFileLogSink(null, verbosity: Given.Verbosity ?? Verbosity.All);
			else
				Then.Logger = new RollingFileLogSink(settings, GetMockLogFileManager(), verbosity: Given.Verbosity ?? Verbosity.All);
		}

		private IRollingFileManager GetMockLogFileManager()
		{
			var logFileManager = MockRepository.GenerateMock<IRollingFileManager>();

			Then.Writer = new StreamWriter(new MemoryStream());
			logFileManager.Stub(x => x.GetStreamWriter()).Return(Then.Writer);
			return logFileManager;
		}

		private void Logging()
		{
			foreach (ILogEntry entry in Given.Entries)
				Then.Logger.Post(entry);
		}

		private void Disposing()
		{
			Then.Logger.Dispose();

			Then.Writer.Flush();
			Then.Writer.BaseStream.Position = 0;

			using (var reader = new StreamReader(Then.Writer.BaseStream))
			{
				Then.Text = reader.ReadToEnd();
			}

			Then.Writer = null;
		}

		private static ILogEntry CreateEntry(string message, Verbosity verbosity = Verbosity.Info)
		{
			using (ShimsContext.Create())
			{
				ShimDateTime.NowGet = () => new DateTime(2015, 12, 16, 0, 0, 0);
				return new LogEntry(message, verbosity);
			}
		}
	}

	public sealed class AsyncFileLoggerThens : IDisposable
	{
		public RollingFileLogSink Logger;
		public StreamWriter Writer;
		public string Text;

		public void Dispose()
		{
			if (Writer != null)
				Writer.Dispose();

			if (Logger != null)
				Logger.Dispose();
		}
	}
}