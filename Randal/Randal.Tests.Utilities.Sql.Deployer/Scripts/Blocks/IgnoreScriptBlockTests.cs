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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.Scripts.Blocks
{
	[TestClass]
	public sealed class IgnoreScriptBlockTests : BaseUnitTest<IgnoreScriptBlockThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidIgnoreScriptBlockWhenCreating()
		{
			Given.Text = "use master\nGO\n";

			When(Creating);

			Then.Object.Should().NotBeNull().And.BeAssignableTo<BaseScriptBlock>();
			Then.Object.IsValid.Should().BeTrue();
			Then.Object.Text.Should().Be("use master\nGO");
		}

		protected override void Creating()
		{
			Then.Object = new IgnoreScriptBlock(Given.Text);
		}
	}

	public sealed class IgnoreScriptBlockThens
	{
		public IgnoreScriptBlock Object;
	}
}