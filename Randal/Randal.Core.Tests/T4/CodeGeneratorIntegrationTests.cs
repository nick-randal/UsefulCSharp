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
using System.Data;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.T4;
using GwtUnit.UnitTest;

namespace Randal.Tests.Core.T4
{
	[TestClass]
	public sealed class CodeGeneratorIntegrationTests : UnitTestBase<CodeGeneratorIntegrationThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveListOfCodeDefintions_WhenGeneratingList()
		{
			Given.SqlCommand = "select system_type_id, name, name as display, name as [description] from sys.types order by name";
			Given.CommandType = CommandType.Text;

			When(GeneratingList);

			Then.Lines.Should().NotBeEmpty();
			Then.Lines[0].Should().NotBe("/*", "should not have thrown exception, but found {0}", string.Join(Environment.NewLine, Then.Lines));
			Then.Lines[0].Should().Be("/// <summary>");
			Then.Lines[1].Should().Be("/// Bigint (127). bigint.");
			Then.Lines[2].Should().Be("/// </summary>");
			Then.Lines[3].Should().Be("[Display(Name = \"bigint\", Description = \"bigint.\")]");
			Then.Lines[4].Should().Be("Bigint = 127,");
			Then.Lines.Last().Should().NotBeEmpty();
			Then.Lines.Last().Should().NotEndWith(",");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValidCode_WhenGeneratingFullCode()
		{
			Given.SqlCommand = "select system_type_id, name, name as display, name as [description] from sys.types order by name";
			Given.CommandType = CommandType.Text;

			When(GeneratingFullCode);

			Then.FullCode.Should().EndWith(ExpectedEndsWith);
		}

		private void GeneratingList()
		{
			IReadOnlyList<DbCodeDefinition> codes = Then.Target.GetCodeDefinitions(Given.SqlCommand, Given.CommandType);
			Then.Lines = codes.ToCodeLines();
		}

		private void GeneratingFullCode()
		{
			string sqlCommand = Given.SqlCommand;
			CommandType commandType = Given.CommandType;

			Then.FullCode = Then.Target.GetCodeDefinitions(sqlCommand, commandType).ToFullyFormattedCode("Examples", "SqlTypes");
		}

		protected override void Creating()
		{
			Then.Target = new CodeGenerator("Data Source=.;Integrated Security=true;Initial Catalog=master;");
		}

		private const string ExpectedEndsWith =
			"/// <summary>\r\n\t\t/// Xml (241). xml.\r\n\t\t/// </summary>\r\n\t\t[Display(Name = \"xml\", Description = \"xml.\")]\r\n\t\tXml = 241\r\n\t}\r\n}";
	}

	public sealed class CodeGeneratorIntegrationThens
	{
		public CodeGenerator Target;
		public IReadOnlyList<string> Lines;
		public string FullCode;
	}
}
