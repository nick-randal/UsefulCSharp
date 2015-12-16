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

using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;

namespace Randal.Tests.Logging
{
	[TestClass]
	public sealed class LogFileTests : UnitTestBase<LogFileThens>
	{
		protected override void OnSetup()
		{
			Given.SizeInBytes = 1024L;
		}

		protected override void OnTeardown()
		{
			Then.Log.Dispose();

			var file = new FileInfo(Given.Path);
			if (file.Exists)
				file.Delete();
		}

		[TestMethod]
		public void ShouldHaveLogFileWithUnknownStateWhenCreating()
		{
			Given.Path = @".\Test.log";

			When(Creating);

			Then.Log.Should().NotBeNull().And.BeAssignableTo<ILogFile>();
			Then.Log.FilePath.Should().EndWith("Test.log");
			Then.State.Should().Be(LogFileState.Unknown);
		}

		[TestMethod]
		public void ShouldHaveFileThanCanReadAndWriteWhenOpeningLog()
		{
			Given.Path = @".\Test.log";

			When(OpeningLog);

			Then.State.Should().Be(LogFileState.OpenAvailable);
			Then.Writer.Should().NotBeNull();
			Then.Writer.BaseStream.Length.Should().Be(1024);
			Then.Writer.BaseStream.Position.Should().Be(0);
			Then.Writer.AutoFlush.Should().BeTrue();
			Then.Writer.BaseStream.CanRead.Should().BeTrue();
			Then.Writer.BaseStream.CanSeek.Should().BeTrue();
			Then.Writer.BaseStream.CanWrite.Should().BeTrue();
		}

		[TestMethod, DeploymentItem(Test.Paths.LoggingFolder, Test.Paths.LoggingFolder)]
		public void ShouldHaveFileReadyToAppendWhenOpeningExistingLog()
		{
			Given.Path = Test.Paths.LoggingFolder + @"\Test_004.log";

			When(OpeningLog);

			Then.State.Should().Be(LogFileState.OpenAvailable);
			Then.Writer.Should().NotBeNull();
			Then.Writer.BaseStream.Length.Should().Be(1024);
		}

		[TestMethod, DeploymentItem(Test.Paths.LoggingFolder, Test.Paths.LoggingFolder)]
		public void ShouldHaveExhaustedFileWhenOpeningExistingLogThatIsFull()
		{
			Given.Path = Test.Paths.LoggingFolder + @"\Test_004.log";
			Given.SizeInBytes = 0;

			When(OpeningLog);

			Then.State.Should().Be(LogFileState.OpenExhausted);
			Then.Writer.Should().NotBeNull();
		}

		[TestMethod]
		public void ShouldHaveLogWithStateUnknownWhenClosingLog()
		{
			Given.Path = @".\Test.log";

			When(OpeningLog, ClosingLog);

			Then.State.Should().Be(LogFileState.Unknown);
			Then.Writer.Should().BeNull();
		}

		protected override void Creating()
		{
			Then.Log = new LogFile(Given.Path, Given.SizeInBytes);
			UpdateState();
		}

		private void OpeningLog()
		{
			Then.Log.Open();
			UpdateState();
		}

		private void ClosingLog()
		{
			Then.Log.Close();
			UpdateState();
		}

		private void UpdateState()
		{
			Then.State = Then.Log.State;
			Then.Writer = Then.Log.GetStreamWriter();
		}
	}

	public sealed class LogFileThens
	{
		public LogFile Log;
		public StreamWriter Writer;
		public LogFileState State;
	}
}