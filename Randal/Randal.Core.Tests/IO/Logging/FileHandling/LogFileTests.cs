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

using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.IO.Logging.FileHandling;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.IO.Logging.FileHandling
{
	[TestClass]
	public sealed class LogFileTests : BaseUnitTest<LogFileThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
			Given.SizeInBytes = 1024L;
		}

		[TestCleanup]
		public void Teardown()
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

			When(Creating, OpeningLog);

			Then.State.Should().Be(LogFileState.OpenAvailable);
			Then.Writer.Should().NotBeNull();
			Then.Writer.BaseStream.Length.Should().Be(1024);
			Then.Writer.BaseStream.Position.Should().Be(0);
			Then.Writer.AutoFlush.Should().BeTrue();
			Then.Writer.BaseStream.CanRead.Should().BeTrue();
			Then.Writer.BaseStream.CanSeek.Should().BeTrue();
			Then.Writer.BaseStream.CanWrite.Should().BeTrue();
		}

		[TestMethod, DeploymentItem(@"TestFiles\IO\Logging", @"TestFiles\IO\Logging")]
		public void ShouldHaveFileReadyToAppendWhenOpeningExistingLog()
		{
			Given.Path = @".\TestFiles\IO\Logging\Test_004.log";

			When(Creating, OpeningLog);

			Then.State.Should().Be(LogFileState.OpenAvailable);
			Then.Writer.Should().NotBeNull();
			Then.Writer.BaseStream.Length.Should().Be(1024);
		}

		[TestMethod, DeploymentItem(@"TestFiles\IO\Logging", @"TestFiles\IO\Logging")]
		public void ShouldHaveExhaustedFileWhenOpeningExistingLogThatIsFull()
		{
			Given.Path = @".\TestFiles\IO\Logging\Test_004.log";
			Given.SizeInBytes = 0;

			When(Creating, OpeningLog);

			Then.State.Should().Be(LogFileState.OpenExhausted);
			Then.Writer.Should().NotBeNull();
		}

		[TestMethod]
		public void ShouldHaveLogWithStateUnknownWhenClosingLog()
		{
			Given.Path = @".\Test.log";

			When(Creating, OpeningLog, ClosingLog);

			Then.State.Should().Be(LogFileState.Unknown);
			Then.Writer.Should().BeNull();
		}

		private void Creating()
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