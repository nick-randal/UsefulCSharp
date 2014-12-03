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

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.Scripts.Blocks
{
	[TestClass]
	public sealed class UnexpectedBlockTests : BaseUnitTest<UnexpectedBlockThens>
	{
		protected override void OnSetup()
		{
			
		}

		[TestMethod, PositiveTest]
		public void ShouldCreateValidInstanceWhenCreatingUnexpectedBlockFromText()
		{
			Given.Text = "--:: unknown\nselect 1\nGO\n";

			When(Creating);

			Then.Object.Should().NotBeNull().And.BeAssignableTo<IScriptBlock>();
			Then.Object.IsValid.Should().BeFalse();
			Then.Object.Keyword.Should().Be("unknown");
			Then.Object.Text.Should().Be("--:: unknown\nselect 1\nGO");
		}

		[TestMethod, PositiveTest]
		public void ShouldLeaveTextUnalteredAndHaveErrorMessageWhenParsingAnUnexpectedBlock()
		{
			Given.Text = "--:: unknown\nselect 1\nGO\n";

			When(Parsing);

			Then.Messages.Should().HaveCount(1);
			Then.Messages.First().Should().Be("Unexpected keyword 'unknown' found for this block.");
		}

		protected override void Creating()
		{
			if(GivensDefined("Keyword") == false)
				Given.Keyword = "unknown";
			
			Then.Object = new UnexpectedBlock(Given.Keyword, Given.Text);
		}

		private void Parsing()
		{
			Then.Messages = Then.Object.Parse();
		}
	}

	public sealed class UnexpectedBlockThens
	{
		public UnexpectedBlock Object;
		public IReadOnlyList<string> Messages;
	}
}