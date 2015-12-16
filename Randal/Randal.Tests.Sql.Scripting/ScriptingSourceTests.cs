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
using Randal.Sql.Scripting;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class ScriptingSourceTests : UnitTestBase<ScriptingSourceThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveInstance_WhenCreating_GivenValidValues()
		{
			Given.SubFolder = "MyFolder";
			Given.SchemaObject = null;

			When(Creating);

			Then.Target.Should().NotBeNull().And.BeAssignableTo<IScriptingSource>();
			Then.Target.SubFolder.Should().Be("MyFolder");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveSchemaObjects_WhenGettingFromSource_GivenSproc()
		{
			Given.SchemaObject = new StoredProcedure(new Database(new Server(), "Test"), "getPatients");

			When(GettingObjects);

			Then.Objects.Should().HaveCount(1);
			Then.Objects.First().Name.Should().Be("getPatients");
		}

		private void GettingObjects()
		{
			Then.Objects = Then.Target.GetScriptableObjects(null, null);
		}

		protected override void Creating()
		{
			Func<IServer, Database, IEnumerable<ScriptSchemaObjectBase>> func =
				(server, database) => new List<ScriptSchemaObjectBase> {Given.SchemaObject};

			Then.Target = new ScriptingSource(Given.SubFolder, func);
		}
	}

	public sealed class ScriptingSourceThens
	{
		public ScriptingSource Target;
		public IEnumerable<ScriptSchemaObjectBase> Objects;
	}
}