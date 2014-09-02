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

using FluentAssertions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Scripting;
using Rhino.Mocks;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class ScriptFormatterTests : BaseUnitTest<ScriptFormatterThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidScriptFormatterWhenCreatingInstace()
		{
			When(Creating);
			Then.Formatter.Should().NotBeNull().And.BeAssignableTo<IScriptFormatter>();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveText_WhenFormatting_GivenStoredProcedure()
		{
			Given.Procedure = "spTest";
			When(Formatting);
			Then.Text.Should()
				.Be("--:: catalog Test\r\n\r\n--:: ignore\r\nuse Test\r\n\r\n--:: pre\r\n" + 
					"exec coreCreateProcedure '[dbo].[spTest]'\r\nGO\r\n\r\n--:: main\r\n" +
					"ALTER procedure [dbo].[spTest]\r\nbegin\r\n\treturn -1\r\nend\r\n\r\n/*\r\n	exec [dbo].[spTest] \r\n*/");
		}

		protected override void Creating()
		{
			var server = MockRepository.GenerateMock<IServer>();
			server.Stub(x => x.GetDependencies(Arg<SqlSmoObject>.Is.NotNull)).Return(new DependencyCollectionNode[0]);
			Then.Formatter = new ScriptFormatter(server);
		}

		private void Formatting()
		{
			ScriptSchemaObjectBase schemaObject;

			if (GivensDefined("Function"))
			{
				schemaObject = new StoredProcedure(new Database(new Server(), "Test"), Given.Procedure)
				{
					Schema = "dbo",
					TextHeader = "create procedure spTest",
					TextBody = "return -1"
				};
			}
			else
			{
				schemaObject = new UserDefinedFunction(new Database(new Server(), "Test"), Given.Function)
				{
					Schema = "dbo",
					TextHeader = "create function getTest",
					TextBody = "return -1"
				};
			}

			Then.Text = Then.Formatter.Format(schemaObject);
		}
	}

	public sealed class ScriptFormatterThens
	{
		public ScriptFormatter Formatter;
		public string Text;
	}
}