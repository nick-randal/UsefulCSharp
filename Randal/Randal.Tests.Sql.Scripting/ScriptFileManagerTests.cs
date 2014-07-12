using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using FluentAssertions;
using Randal.Sql.Scripting;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class ScriptFileManagerTests : BaseUnitTest<ScriptFileManagerThens>
	{
		[TestMethod]
		public void ShouldHaveDirectoryWhenCreatingDirectory()
		{
			Given.DatabaseName = @"Research";
			Given.SubFolder = "Views";

			When(Creating, AddingDirectory);

			Then.Exists.Should().BeTrue();
			Then.Manager.CurrentFolder.Should().Be("");
		}

		private void Creating()
		{
			Then.Manager = new ScriptFileManager(".");
		}

		private void AddingDirectory()
		{
			Then.Exists = Then.Manager.CreateDirectory(Given.DatabaseName, Given.SubFolder);
		}
	}

	public sealed class ScriptFileManagerThens
	{
		public ScriptFileManager Manager;
		public bool Exists;
	}
}
