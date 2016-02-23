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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Strings;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.Strings
{
	[TestClass]
	public sealed class NamedFieldFormatterTests : BaseUnitTest<NamedFieldFormatterThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidFormatterWhenCreatingInstance()
		{
			Given.Text = "Hello world";
			When(Creating);
			Then.Formatter.Should().NotBeNull().And.BeAssignableTo<IStringFormatter>();
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
		public void ShouldHaveBracesWhenFormattingWithEscapedBraces()
		{
			Given.Text = "{{Hello world}}";

			When(Formatting);

			Then.Text.Should().Be("{Hello world}");
		}

		[TestMethod, PositiveTest]
		public void ShouldFormattedValueWhenFormattingBackToBackFields()
		{
			Given.Text = "{part1}{part2}";
			Given.Values = new Dictionary<string, object> {{"part1", "Foo"}, {"part2", "Bar"}};

			When(Formatting);

			Then.Text.Should().Be("FooBar");
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowFormatExcpetionWhenFormattingWithUnescapedClosingBrace()
		{
			Given.Text = "Hey name},";

			ThrowsExceptionWhen(Formatting);

			ThenLastAction.ShouldThrow<FormatException>("Unescaped closing brace found at position 9.");
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowFormatExcpetionWhenFormattingWithUnescapedOpeningBraceAndNoMoreText()
		{
			Given.Text = "Hey {";

			ThrowsExceptionWhen(Formatting);

			ThenLastAction.ShouldThrow<FormatException>("Opening brace with no field name specified at position 5.");
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowFormatExcpetionWhenFormattingWithUnescapedOpeningBrace()
		{
			Given.Text = "Hey {name,";

			ThrowsExceptionWhen(Formatting);

			ThenLastAction.ShouldThrow<FormatException>("Opening brace with field expression 'name,' has no closing brace at position 10.");
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowFormatExcpetionWhenFormattingWithEmptyBraces()
		{
			Given.Text = "{}";

			ThrowsExceptionWhen(Formatting);

			ThenLastAction.ShouldThrow<FormatException>("Opening brace with no field name specified at position 1.");
		}

		protected override void Creating()
		{
			Then.Formatter = new NamedFieldFormatter();
		}

		private void Formatting()
		{
			if(GivensDefined("Values") == false)
				Given.Values = new Dictionary<string, object>();

			Func<string, string> getValueFunc = key => Given.Values[key].ToString();
			Then.Text = Then.Formatter.Parse(Given.Text, getValueFunc);
		}
	}

	public sealed class NamedFieldFormatterThens
	{
		public NamedFieldFormatter Formatter;
		public string Text;
	}
}