using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Strings;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.Strings
{
	[TestClass]
	public sealed class TextTests : BaseUnitTest<TextThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveNonNullText_WhenCreating_GivenNull()
		{
			Given.Value = null;

			When(Creating);

			Then.Text.Should().NotBeNull();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveText_WhenCreating_GivenValue()
		{
			Given.Value = "Lil sumpin sumpin";

			When(Creating);

			Then.Text.Should().Be("Lil sumpin sumpin");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveText_WhenCasting_GivenInteger()
		{
			Given.Integer = 2468;

			When(CastingToText);

			Then.Text.Should().Be("2468");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveRepeatedText_WhenMultiplying_GivenText()
		{
			Given.Value = "Apple";
			Given.Integer = 3;

			When(Multiplying);

			Then.Text.Should().Be("AppleAppleApple");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveIntegerValue_WhenCasting_GivenText()
		{
			Given.Value = "45467";

			When(CastingToInteger);

			Then.Integer.Should().Be(45467);
		}

		[TestMethod, PositiveTest]
		public void ShouldBeEqual_WhenCheckingEquality_GivenSameString()
		{
			Given.Value = "Siamese";
			Given.Other = "Siamese";

			When(CheckingEquality);

			Then.Equality.Should().BeTrue();
		}

		[TestMethod, PositiveTest]
		public void ShouldNotBeEqual_WhenCheckingEquality_GivenSameString()
		{
			Given.Value = "Siamese";
			Given.Other = "Tabby";

			When(CheckingEquality);

			Then.Equality.Should().BeFalse();
		}

		protected override void Creating()
		{
			Then.Text = Given.Value;
		}

		private void CheckingEquality()
		{
			Then.Equality = Then.Text.Equals(Given.Other);
		}

		private void CastingToText()
		{
			Then.Text = (Text)Given.Integer;
		}

		private void CastingToInteger()
		{
			Then.Integer = (int) Then.Text;
		}

		private void Multiplying()
		{
			Then.Text = Then.Text*Given.Integer;
		}
	}

	public sealed class TextThens
	{
		public Text Text;
		public int Integer;
		public bool Equality;
	}
}
