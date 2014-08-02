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
		public void ShouldHaveDirectoryWhenCreatingDirectory()
		{
			Given.DatabaseName = "Research";
			Given.SubFolder = "Views";

			When(AddingDirectory);

			Then.Exists.Should().BeTrue();
			Then.Manager.CurrentFolder.Should().Be(@".\Research\Views");
		}

		protected override void Creating()
		{
			Then.Manager = new ScriptFileManager(".");
		}

		private void AddingDirectory()
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