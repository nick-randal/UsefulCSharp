using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Core.Strings;
using FluentAssertions;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Randal.Tests.Core.Strings
{
	[TestClass]
	public sealed class StringFormatterTests : BaseUnitTest<StringFormatterThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveValidFormatterWhenCreatingInstance()
		{
			Given.Text = "Hello world";
			When(Creating);
			Then.Formatter.Should().NotBeNull().And.BeAssignableTo<IStringFormatter>();
		}

		[TestMethod]
		public void ShouldHaveFormattedStringWhenFormattingWithDictionaryValues()
		{
			Given.Text = "Hello my name is {name} and I am {age} years old.";
			Given.Values = new Dictionary<string, object> { { "name", "Inigo Montoya"}, { "age", 32 } };

			When(Creating, Formatting);

			Then.Text.Should().Be("Hello my name is Inigo Montoya and I am 32 years old.");
		}

		[TestMethod]
		public void ShouldHaveFormattedStringWhenFormattingWithNameValueCollection()
		{
			Given.Text = "Hello my name is {name} and I am {age} years old.";
			Given.Values = new NameValueCollection(); 
			Given.Values.Add("name", "Ignatius Freeley");
			Given.Values.Add("age", "24");

			When(Creating, Formatting);

			Then.Text.Should().Be("Hello my name is Ignatius Freeley and I am 24 years old.");
		}

		[TestMethod]
		public void ShouldHaveFormattedStringWhenFormattingWithObject()
		{
			Given.Text = "Hello my name is {name} and I am {age} years old.";
			Given.Values = new { name = "Slim Shadee", age = 28 };

			When(Creating, Formatting);

			Then.Text.Should().Be("Hello my name is Slim Shadee and I am 28 years old.");
		}

		[TestMethod]
		public void ShouldHaveBracesWhenFormattingWithEscapedBraces()
		{
			Given.Text = "{{Hello world}}";
			Given.Values = new { };

			When(Creating, Formatting);

			Then.Text.Should().Be("{Hello world}");
		}

		[TestMethod]
		public void ShouldFormattedValueWhenFormattingBackToBackFields()
		{
			Given.Text = "{part1}{part2}";
			Given.Values = new { part1 = "Foo", part2 = "Bar" };

			When(Creating, Formatting);

			Then.Text.Should().Be("FooBar");
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ShouldThrowArgumentExceptionWhenCreatingInstanceGivenNullString()
		{
			Given.Text = null;
			When(Creating);
		}

		[TestMethod, ExpectedException(typeof(FormatException))]
		public void ShouldThrowFormatExcpetionWhenFormattingWithUnescapedClosingBrace()
		{
			Given.Text = "Hey name},";
			Given.Values = new { };

			When(Creating, Formatting);
		}

		[TestMethod, ExpectedException(typeof(FormatException))]
		public void ShouldThrowFormatExcpetionWhenFormattingWithUnescapedOpeningBraceAndNoMoreText()
		{
			Given.Text = "Hey {";
			Given.Values = new { };

			When(Creating, Formatting);
		}

		[TestMethod, ExpectedException(typeof(FormatException))]
		public void ShouldThrowFormatExcpetionWhenFormattingWithUnescapedOpeningBrace()
		{
			Given.Text = "Hey {name,";
			Given.Values = new { };

			When(Creating, Formatting);
		}

		[TestMethod, ExpectedException(typeof(FormatException))]
		public void ShouldThrowFormatExcpetionWhenFormattingWithEmptyBraces()
		{
			Given.Text = "{}";
			Given.Values = new { };

			When(Creating, Formatting);
		}

		private void Formatting()
		{
			Then.Text = Then.Formatter.With(Given.Values);
		}

		private void Creating()
		{
			Then.Formatter = new StringFormatter(Given.Text);
		}
	}

	public sealed class StringFormatterThens
	{
		public StringFormatter Formatter;
		public string Text;
	}
}
