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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.T4;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.T4
{
	[TestClass]
	public sealed class StringExtensionTests : BaseUnitTest<StringExtensionThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldConvertLeadingNumber_WhenConverting()
		{
			Given.Text = "45";
			When(Converting);
			Then.Target.Should().Be("Four5");
		}

		[TestMethod, PositiveTest]
		public void ShouldMaintainUnderscores_WhenConverting()
		{
			Given.Text = "_apple_banana";
			When(Converting);
			Then.Target.Should().Be("AppleBanana");
		}


		[TestMethod, PositiveTest]
		public void ShouldRemoveInvalidCharacters_WhenConverting()
		{
			Given.Text = "!@#$%^&*()a";
			When(Converting);
			Then.Target.Should().Be("A");
		}

		protected override void Creating() { }

		private void Converting()
		{
			Then.Target = StringExtensions.ToCSharpPropertyName(Given.Text);
		}
	}

	public sealed class StringExtensionThens
	{
		public string Target;
	}
}
