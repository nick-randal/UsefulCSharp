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
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;

namespace Randal.Tests.Logging
{
	[TestClass, DeploymentItem(Test.Paths.LoggingFolder, Test.Paths.LoggingFolder)]
	public sealed class LogFileManagerTests : UnitTestBase<LogFileManagerTests.Thens>
	{
		[TestMethod, NegativeTest]
		public void ShouldThrowExceptionWhenCreatingGivenNullSettings()
		{
			Given.NullSettings = true;
			WhenLastActionDeferred(Creating);
			ThenLastAction.ShouldThrow<ArgumentNullException>();
		}

		[TestMethod, PositiveTest]
		public void ShouldReturnLogManagerWhenCreating()
		{
			When(Creating);

			Then.Manager.Should().NotBeNull().And.BeAssignableTo<IRollingFileManager>();
			Then.Manager.LogFileName.Should().BeNull();
		}

		[TestMethod, PositiveTest]
		public void ShouldReturnOpenedWriterWhenGettingStreamWriter()
		{
			When(GettingStreamWriter);

			Then.Writer.Should().NotBeNull();
			Then.Manager.LogFileName.Should().EndWith("_001.log");
		}

		[TestMethod, PositiveTest]
		public void ShouldReturnAnotherInstanceOfWriterWhenGettingStreamGivenAnExhaustedLog()
		{
			Given.Size = 2;
			Given.TextToWrite = "Hi";

			When(GettingStreamWriter, WritingText, GettingStreamWriter);

			Then.Writer.Should().NotBeNull();
			Then.Writer.BaseStream.CanWrite.Should().BeTrue();
			Then.Manager.LogFileName.Should().EndWith("_002.log");
		}

		private void GettingStreamWriter()
		{
			Then.Writer = Then.Manager.GetStreamWriter();
		}

		protected override void Creating()
		{
			if (Then.Manager != null)
				return;

			IRollingFileSettings settings = null;
			if (Given.NullSettings == false)
				settings = new RollingFileSettings(Test.Paths.LoggingFolder, "LFMT", Given.Size, false);

			Then.Manager = new RollingFileManager(settings);
		}

		private void WritingText()
		{
			Then.Writer.WriteLine(Given.TextToWrite);
		}

		protected override void OnSetup()
		{
			Given.Size = 1024;
			Given.NullSettings = false;
		}

		protected override void OnTeardown()
		{
			if (Then.Manager == null)
				return;

			var fileName = Then.Manager.LogFileName;

			Then.Manager.Dispose();
			Then.Writer = null;
			Then.Manager = null;

			if (File.Exists(fileName))
				File.Delete(fileName);
		}

		public sealed class Thens
		{
			public RollingFileManager Manager;
			public StreamWriter Writer;
		}
	}
}