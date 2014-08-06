using System;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Scripting;
using System.Collections.Generic;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class ScriptingSourceTests : BaseUnitTest<ScriptingSourceThens>
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
				(server, database) => new List<ScriptSchemaObjectBase> { Given.SchemaObject };

			Then.Target = new ScriptingSource(Given.SubFolder, func);
		}
	}

	public sealed class ScriptingSourceThens
	{
		public ScriptingSource Target;
		public IEnumerable<ScriptSchemaObjectBase> Objects;
	}
}
