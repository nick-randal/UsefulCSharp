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
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;
using Randal.Sql.Scripting;
using Rhino.Mocks;
using Scripter = Randal.Sql.Scripting.Scripter;
using Randal.Tests.Sql.Scripting.Support;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class ScripterTests : UnitTestBase<ScripterThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidInstance_WhenCreating()
		{
			When(Creating);

			Then.Target.Should().NotBeNull();
			Then.Target.Sources.Should().BeEmpty();
			Then.Target.IncludedDatabases.Should().BeEmpty();
			Then.Target.ExcludedDatabases.Should().BeEmpty();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveListItems_WhenAddingSources()
		{
			Given.Sources = new[] { new ScriptingSource("Temp", (srvr, db) => new ScriptSchemaObjectBase[0]) };

			When(AddingSources);

			Then.Target.Sources.Should().HaveCount(1);
			Then.Target.Sources[0].SubFolder.Should().Be("Temp");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveListItems_WhenAddingDatabases_ExcludedDatabases()
		{
			Given.ExcludedDatabases = new[] { "tempdb", "msdb" };

			When(AddingDatabases);

			Then.Target.ExcludedDatabases.Should().HaveCount(2);
			Then.Target.ExcludedDatabases[0].Should().Be("tempdb");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveListItems_WhenAddingDatabases_GivenIncludedDatabases()
		{
			Given.IncludedDatabases = new[] { "master", "Northwnd" };

			When(AddingDatabases);

			Then.Target.IncludedDatabases.Should().HaveCount(2);
			Then.Target.IncludedDatabases[0].Should().Be("master");
		}

		[TestMethod, PositiveTest]
		public void ShouldWriteScriptFile_WhenDumpingScripts()
		{
			Given.Sources = new[]
			{
				new ScriptingSource("Sprocs", (srvr, db) => new ScriptSchemaObjectBase[]
				{
					new StoredProcedure(db, "mySp") { TextMode = false, IsEncrypted = false }
				})
			};
			Given.Databases = new[] { new Database(new Server("."), "Test_Randal_Sql") };

			When(AddingSources, DumpingScripts);

			Then.MockScriptFileManager.AssertWasCalled(x =>
				x.WriteScriptFile(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Equal("dbo.mySp"), Arg<string>.Is.Anything)
			);
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenDumpingScripts_GivenNoSources()
		{
			WhenLastActionDeferred(DumpingScripts);

			ThenLastAction.ShouldThrow<InvalidOperationException>("Sources need to be setup prior to dumping scripts.");
		}

		protected override void OnSetup()
		{
			ServerSetup.Go();
		}

		private void DumpingScripts()
		{
			Then.MockServer.Stub(x => x.GetDatabases()).Return(new List<Database> { new Database(new Server(), "Test_Randal_Sql") }.AsEnumerable());

			Then.Target.DumpScripts();
		}

		private void AddingDatabases()
		{
			if (GivensDefined("IncludedDatabases"))
				Then.Target.IncludeTheseDatabases(Given.IncludedDatabases);
			else
				Then.Target.ExcludedTheseDatabases(Given.ExcludedDatabases);
		}

		private void AddingSources()
		{
			Then.Target.AddSources(Given.Sources);
		}

		protected override void Creating()
		{
			Then.MockServer = MockRepository.GenerateMock<IServer>();
			if (GivensDefined("Databases"))
			{
				Database[] databases = Given.Databases;
				Then.MockServer.Stub(x => x.GetDatabases()).Return(databases.ToList().AsEnumerable());
			}

			Then.MockServer.Stub(x => x.Name).Return(".");

			Then.MockScriptFileManager = MockRepository.GenerateMock<IScriptFileManager>();
			Then.MockLogger = MockRepository.GenerateMock<ILogger>();
			Then.MockScriptFormatter = MockRepository.GenerateMock<IScriptFormatter>();
			Then.Target = new Scripter(Then.MockServer, Then.MockScriptFileManager, Then.MockLogger, Then.MockScriptFormatter);
		}
	}

	public sealed class ScripterThens
	{
		public Scripter Target;
		public IServer MockServer;
		public IScriptFileManager MockScriptFileManager;
		public ILogger MockLogger;
		public IScriptFormatter MockScriptFormatter;
	}
}