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
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.T4
{
	[TestClass]
	public sealed class CodeGeneratorIntegrationTests : BaseUnitTest<CodeGeneratorIntegrationThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveListOfCodeDefintions_WhenGeneratingList()
		{
			Given.SqlCommand = "select system_type_id, name, name as display, name as [description] from sys.types order by name";
			Given.CommandType = CommandType.Text;

			When(GeneratingList);

			Then.Lines.Should().NotBeEmpty();
			Then.Lines[0].Should().NotBe("/*", "should not have thrown exception, but found {0}", string.Join(Environment.NewLine, Then.Lines));
			Then.Lines[0].Should().Be("[Display(Name = \"bigint\", Description = \"bigint\")]");
			Then.Lines[1].Should().Be("Bigint = 127,");
			Then.Lines.Last().Should().NotBeEmpty();
			Then.Lines.Last().Should().NotEndWith(",");
		}

		private void GeneratingList()
		{
			IReadOnlyList<DbCodeDefinition> codes = Then.Target.GetCodeDefinitions(Given.SqlCommand, Given.CommandType);
			Then.Lines = codes.ToCodeLines();
		}

		protected override void Creating()
		{
			Then.Target = new CodeGenerator("Data Source=.;Integrated Security=true;Initial Catalog=master;");
		}
	}

	public sealed class CodeGeneratorIntegrationThens
	{
		public CodeGenerator Target;
		public IReadOnlyList<string> Lines;
	}
}
