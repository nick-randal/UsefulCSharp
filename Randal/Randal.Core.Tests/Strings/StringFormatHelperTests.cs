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
using System.Collections.Specialized;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Strings;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.Strings
{
	[TestClass]
	public sealed class StringFormatHelperTests : BaseUnitTest<StringFormatHelperThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveFormattedStringWhenFormattingWithNameValueCollection()
		{
			Given.Text = "Hello my name is {name} and I am {age} years old.";
			Given.Values = new NameValueCollection();
			Given.Values.Add("name", "Ignatius Freeley");
			Given.Values.Add("age", "24");

			When(Formatting);

			Then.Text.Should().Be("Hello my name is Ignatius Freeley and I am 24 years old.");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveFormattedStringWhenFormattingWithObject()
		{
			Given.Text = "Hello my name is {name} and I am {age} years old.";
			Given.Values = new {name = "Slim Shadee", age = 28};

			When(Formatting);

			Then.Text.Should().Be("Hello my name is Slim Shadee and I am 28 years old.");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveFormattedStringWhenFormattingWithDictionaryValues()
		{
			Given.Text = "Hello my name is {name} and I am {age} years old.";
			Given.Values = new Dictionary<string, object> {{"name", "Inigo Montoya"}, {"age", 32}};

			When(Formatting);

			Then.Text.Should().Be("Hello my name is Inigo Montoya and I am 32 years old.");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveMonetaryFormatWhenFormattingGivenSpecialFormat()
		{
			Given.Text = "show me the {money:c}";
			Given.Values = new { money = 23.99m };

			When(Formatting);

			Then.Text.Should().Be("show me the $23.99");
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowArgumentExceptionWhenCreatingInstanceGivenNullString()
		{
			Given.Text = null;
			ThrowsExceptionWhen(Creating);

			ThenLastAction.ShouldThrow<ArgumentNullException>();
		}

		protected override void Creating()
		{
			Then.FormatHelper = new StringFormatHelper(Given.Text);
		}

		private void Formatting()
		{
			Then.Text = Then.FormatHelper.With(Given.Values);
		}
	}

	public sealed class StringFormatHelperThens
	{
		public StringFormatHelper FormatHelper;
		public string Text;
	}
}