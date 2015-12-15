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

using FluentAssertions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Scripting;
using Randal.Tests.Sql.Scripting.Support;
using Rhino.Mocks;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class ScriptFormatterTests : UnitTestBase<ScriptFormatterTests.Thens>
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
			Given.Procedure = "mySp";

			When(Formatting);

			Then.Text.Should()
				.Be("--:: catalog Test_Randal_Sql\r\n\r\n--:: ignore\r\nuse Test_Randal_Sql\r\n\r\n--:: pre\r\n" + 
					"exec coreCreateProcedure 'mySp', 'dbo'\r\nGO\r\n\r\n--:: main\r\n" +
					"ALTER PROCEDURE [dbo].[mySp]\r\nAS\r\n\r\nbegin\r\n\treturn -1\r\nend\r\n\r\n/*\r\n	exec [dbo].[mySp] \r\n*/");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveText_WhenFormatting_GivenUserDefinedFunction()
		{
			Given.Function = "'myFunc'";

			When(Formatting);

			Then.Text.Should()
				.Be("--:: catalog Test_Randal_Sql\r\n\r\n--:: ignore\r\nuse Test_Randal_Sql\r\n\r\n--:: pre\r\n" +
				"exec coreCreateFunction 'myFunc', 'dbo', 'scalar'\r\nGO\r\n\r\n--:: main\r\n" +
				"ALTER FUNCTION [dbo].[myFunc]()\r\nRETURNS [int] WITH EXECUTE AS CALLER\r\nAS \r\n\r\nbegin return(-1); end\r\n\r\n/*\r\n	select [dbo].[myFunc]()\r\n*/");
		}

		protected override void OnSetup()
		{
			ServerSetup.Go();
		}

		protected override void Creating()
		{
			Then.Formatter = new ScriptFormatter();
		}

		private void Formatting()
		{
			ScriptSchemaObjectBase schemaObject;
			//var server = MockRepository.GenerateMock<IServer>();
			//server.Stub(x => x.GetDependencies(Arg<SqlSmoObject>.Is.NotNull)).Return(new DependencyCollectionNode[0]);

			/*
			if (GivensDefined("Function"))
			{
				schemaObject = new UserDefinedFunction(new Database(new Server("."), "Test_Randal_Sql"), Given.Function)
				{
					TextMode = false,
					DataType = DataType.Int,
					FunctionType = UserDefinedFunctionType.Scalar,
					ImplementationType = ImplementationType.TransactSql,
					TextBody = "begin return -1 end"
				};
			}
			else
			{
				schemaObject = new StoredProcedure(new Database(new Server("."), "Test_Randal_Sql"), Given.Procedure)
				{
					Schema = "dbo",
					TextHeader = "create procedure getTest",
					TextBody = "return -1"
				};
			}*/

			var server = new Server(".");
			var database = server.Databases["Test_Randal_Sql"];
			
			if (GivensDefined("Function"))
				schemaObject = database.UserDefinedFunctions["myFunc", "dbo"];
			else
				schemaObject = database.StoredProcedures["mySp", "dbo"];

			Then.Text = Then.Formatter.Format(new ScriptableObject(null, schemaObject));
		}

		public sealed class Thens
		{
			public ScriptFormatter Formatter;
			public string Text;
		}
	}
}