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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.Scripts.Blocks
{
	[TestClass]
	public sealed class NeedBlockTests : UnitTestBase<NeedBlockThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveInvalidBlock_WhenCreating()
		{
			When(Creating);

			Then.Target.Should().BeAssignableTo<BaseScriptBlock>();
			Then.Target.IsValid.Should().BeFalse();
			Then.Target.Keyword.Should().Be("need");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValidBlock_WhenParsing_GivenValidInput()
		{
			Given.Text = "A, B, C.sql,,D";

			When(Parsing);

			Then.Target.IsValid.Should().BeTrue();
			Then.Target.Files.Should().HaveCount(4);
			Then.Target.Files[0].Should().Be("A");
			Then.Target.Files[1].Should().Be("B");
			Then.Target.Files[2].Should().Be("C");
			Then.Target.Files[3].Should().Be("D");
		}

		[TestMethod, PositiveTest]
		public void ShouldBeInvalid_WhenParsing_GivenListOfInvalidFileNames()
		{
			Given.Text = "File*A, File?B, Procedures\\ReadAll";

			When(Parsing);

			Then.Target.IsValid.Should().BeFalse();
			Then.Messages.Should().HaveCount(3);
		}

		[TestMethod, PositiveTest]
		public void ShouldIgnoreAnythingOnNextLine_WhenParsing_GivenMultipleLines()
		{
			Given.Text = @"ScriptA
-- this should be ignored";

			When(Parsing);

			Then.Target.IsValid.Should().BeTrue();
			Then.Target.Files.Should().HaveCount(1);
			Then.Target.Files.First().Should().Be("ScriptA");
		}

		private void Parsing()
		{
			Then.Messages = Then.Target.Parse();
		}

		protected override void Creating()
		{
			Then.Target = new NeedBlock(GivensDefined("Text") ? Given.Text : string.Empty);
		}
	}

	public sealed class NeedBlockThens
	{
		public NeedBlock Target;
		public IReadOnlyList<string> Messages;
	}
}