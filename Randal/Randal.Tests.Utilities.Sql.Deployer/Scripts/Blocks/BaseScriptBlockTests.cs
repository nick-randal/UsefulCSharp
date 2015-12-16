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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.Scripts.Blocks
{
	[TestClass]
	public sealed class BaseScriptBlockTests : UnitTestBase<BaseScriptBlockThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveNonEmptyTextAfterInstantiation()
		{
			Given.Keyword = "\nignore\n";
			Given.Text = "\nuse master\n";

			When(Creating);

			Then.Object.Should().BeAssignableTo<BaseScriptBlock>();
			Then.Object.IsValid.Should().BeFalse();
			Then.Object.Keyword.Should().Be("ignore");
			Then.Object.Text.Should().Be("use master");
		}

		[TestMethod, NegativeTest]
		public void ShouldHaveEmptyStrings_WhenCreating_GivenNullValues()
		{
			Given.Keyword = null;
			Given.Text = null;

			When(Creating);

			Then.Object.IsValid.Should().BeFalse();
			Then.Object.Keyword.Should().BeEmpty();
			Then.Object.Text.Should().BeEmpty();
		}

		[TestMethod, NegativeTest]
		public void ShouldHaveEmptyListOfMessagesAndBeInvalid_WhenParsing()
		{
			Given.Keyword = "invalid";
			Given.Text = "--:: it doesn't really matter for a mock derivation";

			When(Parsing);

			Then.Object.IsValid.Should().BeFalse();
			Then.Messages.Should().BeEmpty();
		}

		private void Parsing()
		{
			Then.Messages = Then.Object.Parse();
		}

		protected override void Creating()
		{
			Then.Object = new DerivedBaseScriptBlock(Given.Keyword, Given.Text);
		}
	}

	public sealed class DerivedBaseScriptBlock : BaseScriptBlock
	{
		public DerivedBaseScriptBlock(string keyword, string text)
			: base(keyword, text)
		{
		}
	}

	public sealed class BaseScriptBlockThens
	{
		public DerivedBaseScriptBlock Object;
		public IReadOnlyList<string> Messages;
	}
}