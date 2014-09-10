using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.QuickXml;
using Rhino.Mocks.Constraints;
using Sprache;
using System;

namespace Randal.Tests.QuickXml
{
	[TestClass]
	public sealed class QuickXmlParserDefinitionTests : BaseUnitTest<QuickXmlParserDefinitionThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveItem_WhenParsingElement_GivenValidText()
		{
			Given.Text = "\t \t x:Test.Element";

			When(ParsingElement);

			Then.Item.Should().NotBeNull().And.BeAssignableTo<QElement>();
			Then.Item.Depth.Should().Be(4);
			Then.Item.Name.Should().Be("x:Test.Element");
			Then.Item.Type.Should().Be(XmlNodeType.Element);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveItem_WhenParsingComment_GivenValidText()
		{
			Given.Text = "  !this is a comment!";

			When(ParsingComment);

			Then.Item.Should().NotBeNull().And.BeAssignableTo<QComment>();
			Then.Item.Depth.Should().Be(2);
			Then.Item.Value.Should().Be("this is a comment!");
			Then.Item.Type.Should().Be(XmlNodeType.Comment);
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenAccessingName_GivenContent()
		{
			Then.Item = new QContent(0, "Test");

			Action a = () => { var value = Then.Item.Name; };
			a.ShouldThrow<NotSupportedException>();
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenAccessingName_GivenData()
		{
			Then.Item = new QData(0, "Test");

			Action a = () => { var value = Then.Item.Name; };
			a.ShouldThrow<NotSupportedException>();
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenAccessingName_GivenComment()
		{
			Then.Item = new QComment(0, "Test");

			Action a = () => { var value = Then.Item.Name; };
			a.ShouldThrow<NotSupportedException>();
		}

		private void ParsingElement()
		{
			Then.Item = QuickXmlParserDefinition.Element.Parse((string)Given.Text);
		}

		private void ParsingComment()
		{
			Then.Item = QuickXmlParserDefinition.Comment.Parse((string)Given.Text);
		}

		protected override void Creating() { }
	}

	public sealed class QuickXmlParserDefinitionThens
	{
		public IQuickXmlItem Item;
	}
}
