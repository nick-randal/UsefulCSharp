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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Scripts;

namespace Randal.Tests.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class NeedBlockTests : BaseUnitTest<NeedBlockThens>
	{
		protected override void OnSetup()
		{
			Given.Text = string.Empty;
		}

		[TestMethod]
		public void ShouldHaveInvalidBlockWhenCreating()
		{
			When(Creating);

			Then.Object.Should().BeAssignableTo<BaseScriptBlock>();
			Then.Object.IsValid.Should().BeFalse();
			Then.Object.Keyword.Should().Be("need");
		}

		[TestMethod]
		public void ShouldHaveValidBlockWhenParsingGivenValidInput()
		{
			Given.Text = "A, B, C.sql,,D";

			When(Creating, WhenParsing);

			Then.Object.IsValid.Should().BeTrue();
			Then.Object.Files.Should().HaveCount(4);
			Then.Object.Files[0].Should().Be("A");
			Then.Object.Files[1].Should().Be("B");
			Then.Object.Files[2].Should().Be("C");
			Then.Object.Files[3].Should().Be("D");
		}

		[TestMethod]
		public void ShouldBeInvalidWhenParsingGivenListOfInvalidFileNames()
		{
			Given.Text = "File*A, File?B, Procedures\\ReadAll";

			When(Creating, WhenParsing);

			Then.Object.IsValid.Should().BeFalse();
			Then.Messages.Should().HaveCount(3);
		}

		private void WhenParsing()
		{
			Then.Messages = Then.Object.Parse();
		}

		private void Creating()
		{
			Then.Object = new NeedBlock(Given.Text);
		}
	}

	public sealed class NeedBlockThens
	{
		public NeedBlock Object;
		public IReadOnlyList<string> Messages;
	}
}