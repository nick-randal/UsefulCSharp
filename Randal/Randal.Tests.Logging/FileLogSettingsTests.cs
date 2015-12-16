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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;

namespace Randal.Tests.Logging
{
	[TestClass]
	public sealed class FileLogSettingsTests : UnitTestBase<FileLogSettingsThens>
	{
		[TestMethod, DeploymentItem(Test.Paths.LoggingFolder, Test.Paths.LoggingFolder)]
		public void ShouldHaveFileLoggerWhenCreatingGivenValidValues()
		{
			Given.BasePath = Test.Paths.LoggingFolder;
			Given.BaseFileName = "Test";
			Given.FileSize = 1024;
			Given.TruncateRepeatingLines = true;

			When(Creating);

			Then.Settings.Should().NotBeNull().And.BeAssignableTo<IRollingFileSettings>();
			Then.Settings.BasePath.Should().Be(Test.Paths.LoggingFolder);
			Then.Settings.BaseFileName.Should().Be("Test");
			Then.Settings.FileSize.Should().Be(1024);
			Then.Settings.ShouldTruncateRepeatingLines.Should().BeTrue();
		}

		[TestMethod, ExpectedException(typeof (ArgumentException))]
		public void ShouldThrowExceptionWhenCreatingGivenNullBasePath()
		{
			Given.BasePath = null;
			Given.BaseFileName = "Test";
			Given.FileSize = 1024;
			Given.TruncateRepeatingLines = true;

			When(Creating);
		}

		[TestMethod, DeploymentItem(Test.Paths.LoggingFolder, Test.Paths.LoggingFolder)]
		public void ShouldHaveEmptyBaseFileNameWhenCreatingGivenNullBaseFileName()
		{
			Given.BasePath = Test.Paths.LoggingFolder;
			Given.BaseFileName = null;
			Given.FileSize = -1;
			Given.TruncateRepeatingLines = true;

			When(Creating);

			Then.Settings.BaseFileName.Should().BeEmpty();
			Then.Settings.FileSize.Should().Be(RollingFileSettings.FiveMegabytes);
		}

		protected override void Creating()
		{
			Then.Settings = new RollingFileSettings(Given.BasePath, Given.BaseFileName, Given.FileSize,
				Given.TruncateRepeatingLines);
		}
	}

	public sealed class FileLogSettingsThens
	{
		public RollingFileSettings Settings;
	}
}