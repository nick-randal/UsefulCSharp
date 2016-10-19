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

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GwtUnit.UnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.T4;

namespace Randal.Tests.Core.T4
{
	[TestClass]
	public sealed class CodeGeneratorTests : UnitTestBase<CodeGeneratorThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidInstance_WhenCreating()
		{
			When(Creating);

			Then.Target.Should().NotBeNull().And.BeAssignableTo<ICodeGenerator>();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveListOfCodeDefintions_WhenGeneratingList()
		{
			Given.CodeDefinitions = new[]
			{
				new DbCodeDefinition("2", "Hazy", "Component Hazy", "What were we thinking?", 1),
				new DbCodeDefinition("1", "Visible", "Component Visible", "Now you see it."),
				new DbCodeDefinition("0", "Hidden", "Component Hidden", "Now you don't.")
			};

			When(GeneratingList);

			Then.Lines.Should().HaveCount(17);
			Then.Lines[0].Should().Be("/// <summary>");
			Then.Lines[1].Should().Be("/// Hazy (2). What were we thinking?");
			Then.Lines[2].Should().Be("/// </summary>");
			Then.Lines[3].Should().Be("[Obsolete, Display(Name = \"Component Hazy\", Description = \"What were we thinking?\")]");
			Then.Lines[4].Should().Be("Hazy = 2,");
			Then.Lines[5].Should().Be("");
			Then.Lines[6].Should().Be("/// <summary>");
			Then.Lines[7].Should().Be("/// Visible (1). Now you see it.");
			Then.Lines[8].Should().Be("/// </summary>");
			Then.Lines[9].Should().Be("[Display(Name = \"Component Visible\", Description = \"Now you see it.\")]");
			Then.Lines[10].Should().Be("Visible = 1,");
			
			Then.Lines.Last().Should().NotBeEmpty();
			Then.Lines.Last().Should().NotEndWith(",");
		}

		private void GeneratingList()
		{
			IReadOnlyList<DbCodeDefinition> codes = Given.CodeDefinitions;
			Then.Lines = codes.ToCodeLines();
		}

		protected override void Creating()
		{
			Then.Target = new CodeGenerator("Data Source=.;Integrated Security=true;Initial Catalog=master;");
		}
	}

	public sealed class CodeGeneratorThens
	{
		public CodeGenerator Target;
		public IReadOnlyList<string> Lines;
	}
}
