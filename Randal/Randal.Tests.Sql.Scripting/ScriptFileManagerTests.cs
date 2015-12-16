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
using Randal.Sql.Scripting;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class ScriptFileManagerTests : UnitTestBase<ScriptFileManagerThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveDirectory_WhenCreatingDirectory()
		{
			Given.DatabaseName = "Research";
			Given.SubFolder = "Views";

			When(CreatingDirectory);

			Then.Exists.Should().BeTrue();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveFile_WhenWritingScript_GivenText()
		{
			Given.DatabaseName = "Research";
			Given.SubFolder = "Views";
			Given.File = "Test";
			Given.Text = "Select 1;";

			When(CreatingDirectory, WritingScript);

			Then.Exists.Should().BeTrue();
		}
		
		private void WritingScript()
		{
			var path = Then.Manager.WriteScriptFile(Given.DatabaseName, Given.SubFolder, Given.File, Given.Text);
			Then.Exists = new FileInfo(path).Exists;
		}

		protected override void Creating()
		{
			Then.Manager = new ScriptFileManager(".");
		}

		private void CreatingDirectory()
		{
			var path = Then.Manager.SetupScriptDirectory(Given.DatabaseName, Given.SubFolder);
			Then.Exists = new DirectoryInfo(path).Exists;
		}
	}

	public sealed class ScriptFileManagerThens
	{
		public ScriptFileManager Manager;
		public bool Exists;
	}
}