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
using System.IO;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.IO.Logging.FileHandling;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.IO.Logging.FileHandling
{
	[TestClass]
	public sealed class LogFolderTests : BaseUnitTest<LogFolderThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();

			Given.FolderDoesNotExist = false;
		}

		[TestMethod]
		public void ShouldHaveValidLogFolderWhenCreatingGivenValidPath()
		{
			Given.Path = @".\LogTest";
			Given.BaseFileName = "System";
			Given.FolderDoesNotExist = true;

			When(Creating);

			Then.Folder.Should().NotBeNull().And.BeAssignableTo<ILogFolder>();
			Then.FolderExists.Should().BeFalse();
		}

		[TestMethod]
		public void ShouldHaveFileSystemFolderWhenVerifyingFolderGivenFolderDoesNotExist()
		{
			Given.Path = @".\LogTest";
			Given.BaseFileName = "System";
			Given.FolderDoesNotExist = true;
			
			When(Creating, VerifyingFolder);

			Then.FolderVerified.Should().BeTrue();
			Then.FolderExists.Should().BeTrue();
		}

		[TestMethod, DeploymentItem(@"TestFiles\IO\Logging", @"TestFiles\IO\Logging")]
		public void ShouldHaveFilePathForExistingFileWhenGetNextFilePathGivenAnExistingLogFile()
		{
			Given.Path = @".\TestFiles\IO\Logging";
			Given.BaseFileName = "Test";

			When(Creating, GettingNextFilePath);

			Then.FilePath.Should().EndWith("Test_004.log");
			Then.FileExists.Should().BeTrue();
		}

		[TestMethod, DeploymentItem(@"TestFiles\IO\Logging", @"TestFiles\IO\Logging")]
		public void ShouldHaveSubsequentFilePathForExistingFileWhenGettingNextFilePathGivenTwoAttempts()
		{
			Given.Path = @".\TestFiles\IO\Logging";
			Given.BaseFileName = "Test";

			When(Creating, GettingNextFilePath, GettingNextFilePath);

			Then.FilePath.Should().EndWith("Test_005.log");
			Then.FileExists.Should().BeFalse();
		}

		[TestMethod]
		public void ShouldHaveFilePathWithIndexOneWhenGettingNextFilePathGivenNoFiles()
		{
			Given.Path = ".";
			Given.BaseFileName = "Test";

			When(Creating, GettingNextFilePath);

			Then.FilePath.Should().EndWith("Test_001.log");
			Then.FileExists.Should().BeFalse();
		}

		[TestMethod]
		public void ShouldHaveFallbackFilePathWhenGettingFallbackFilePath()
		{
			Given.FakeGuid = "3364dd9a-5fd3-4cd9-a5b7-96a8f82bed08";
			Given.Path = ".";
			Given.BaseFileName = "Test";

			When(Creating, GettingFallbackFilePath);

			Then.FilePath.Should().EndWith("Test_3364dd9a-5fd3-4cd9-a5b7-96a8f82bed08.log");
			Then.FileExists.Should().BeFalse();
		}

		private void GettingNextFilePath()
		{
			Then.FilePath = Then.Folder.GetNextLogFilePath();
			Then.FileExists = new FileInfo(Then.FilePath).Exists;
		}

		private void GettingFallbackFilePath()
		{
			using (ShimsContext.Create())
			{
				ShimGuid.NewGuid = () => new Guid(Given.FakeGuid);
				Then.FilePath = Then.Folder.GetFallbackFilePath();
			}
		}

		private void Creating()
		{
			if (Given.FolderDoesNotExist)
			{	
				var directory = new DirectoryInfo(Given.Path);
				if (directory.Exists)
					directory.Delete(true);	
			}

			Then.Folder = new LogFolder(Given.Path, Given.BaseFileName);
			Then.FolderExists = new DirectoryInfo(Given.Path).Exists;
		}

		private void VerifyingFolder()
		{
			Then.FolderVerified = Then.Folder.VerifyOrCreate();
			Then.FolderExists = new DirectoryInfo(Given.Path).Exists;
		}
	}

	public sealed class LogFolderThens
	{
		public LogFolder Folder;
		public bool FolderVerified, FolderExists, FileExists;
		public string FilePath;
	}
}