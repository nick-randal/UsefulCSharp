﻿// Useful C#
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
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;

namespace Randal.Tests.Logging
{
	[TestClass, DeploymentItem(Test.Paths.LoggingFolder, Test.Paths.LoggingFolder)]
	public sealed class LogFileManagerTests : BaseUnitTest<LogFileManagerThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();

			Given.Size = 1024;
			Given.NullSettings = false;
		}

		[TestCleanup]
		public void Teardown()
		{
			if (Then.Manager == null)
				return;

			Then.Manager.Dispose();
			Then.Writer = null;
			Then.Manager = null;
		}

		[TestMethod, ExpectedException(typeof (ArgumentNullException))]
		public void ShouldThrowExceptionWhenCreatingGivenNullSettings()
		{
			Given.NullSettings = true;

			When(Creating);
		}

		[TestMethod]
		public void ShouldReturnLogManagerWhenCreating()
		{
			When(Creating);

			Then.Manager.Should().NotBeNull().And.BeAssignableTo<ILogFileManager>();
			Then.Manager.LogFileName.Should().BeNull();
		}

		[TestMethod]
		public void ShouldReturnOpenedWriterWhenGettingStreamWriter()
		{
			When(Creating, GettingStreamWriter);

			Then.Writer.Should().NotBeNull();
			Then.Manager.LogFileName.Should().EndWith("_001.log");
		}

		[TestMethod]
		public void ShouldReturnAnotherInstanceOfWriterWhenGettingStreamGivenAnExhaustedLog()
		{
			Given.Size = 2;
			Given.TextToWrite = "Hi";

			When(Creating, GettingStreamWriter, WritingText, GettingStreamWriter);

			Then.Writer.Should().NotBeNull();
			Then.Writer.BaseStream.CanWrite.Should().BeTrue();
			Then.Manager.LogFileName.Should().EndWith("_002.log");
		}

		private void GettingStreamWriter()
		{
			Then.Writer = Then.Manager.GetStreamWriter();
		}

		private void Creating()
		{
			if (Then.Manager != null)
				return;

			IFileLoggerSettings settings = null;
			if (Given.NullSettings == false)
				settings = new FileLoggerSettings(Test.Paths.LoggingFolder, "LFMT", Given.Size, false);

			Then.Manager = new LogFileManager(settings);
		}

		private void WritingText()
		{
			Then.Writer.WriteLine(Given.TextToWrite);
		}
	}

	public sealed class LogFileManagerThens
	{
		public LogFileManager Manager;
		public StreamWriter Writer;
	}
}