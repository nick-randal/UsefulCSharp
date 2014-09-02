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
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Scripting;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class ScriptFileManagerTests : BaseUnitTest<ScriptFileManagerThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveDirectory_WhenCreatingDirectory()
		{
			Given.DatabaseName = "Research";
			Given.SubFolder = "Views";

			When(CreatingDirectory);

			Then.Exists.Should().BeTrue();
			Then.Manager.CurrentFolder.Should().Be(@".\Research\Views");
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

		[TestMethod, PositiveTest]
		public void ShouldHaveFile_WhenWritingScriptAsync_GivenText()
		{
			Given.DatabaseName = "Research";
			Given.SubFolder = "Views";
			Given.File = "Test";
			Given.Text = "Select 1;";

			When(CreatingDirectory, WritingScriptAsync);

			Then.Exists.Should().BeTrue();
		}

		private async void WritingScriptAsync()
		{
			await Then.Manager.WriteScriptFileAsync(Given.File, Given.Text);
			Then.Exists = new FileInfo(".\\" + Then.Manager.CurrentFolder + "\\" + Given.File + ".sql").Exists;
		}

		private void WritingScript()
		{
			Then.Manager.WriteScriptFile(Given.File, Given.Text);
			Then.Exists = new FileInfo(".\\" + Then.Manager.CurrentFolder + "\\" + Given.File + ".sql").Exists;
		}

		protected override void Creating()
		{
			Then.Manager = new ScriptFileManager(".");
		}

		private void CreatingDirectory()
		{
			Then.Manager.CreateDirectory(Given.DatabaseName, Given.SubFolder);

			var directory = new DirectoryInfo(Then.Manager.CurrentFolder);
			Then.Exists = directory.Exists;
		}
	}

	public sealed class ScriptFileManagerThens
	{
		public ScriptFileManager Manager;
		public bool Exists;
	}
}