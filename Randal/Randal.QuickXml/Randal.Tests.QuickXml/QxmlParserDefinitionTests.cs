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
	public sealed class QxmlParserDefinitionTests : BaseUnitTest<QxmlParserDefinitionThens>
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
		public void ShouldHaveXElement_WhenParsingElement_GivenValidText()
		{
			Given.Text = "\t \t Test.Element";

			When(ParsingElement, Converting);

			Then.Node.Should().BeAssignableTo<XElement>();
			((XElement) Then.Node).Name.LocalName.Should().Be("Test.Element");
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

		[TestMethod, PositiveTest]
		public void ShouldHaveXComment_WhenParsingComment_GivenValidText()
		{
			Given.Text = "\t \t !this is a comment!";

			When(ParsingComment, Converting);

			Then.Node.Should().BeAssignableTo<XComment>();
			((XComment)Then.Node).Value.Should().Be("this is a comment!");
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

		private void Converting()
		{
			Then.Node = Then.Item.ToNode();
		}

		private void ParsingElement()
		{
			Then.Item = QxmlParserDefinition.Element.Parse((string)Given.Text);
		}

		private void ParsingComment()
		{
			Then.Item = QxmlParserDefinition.Comment.Parse((string)Given.Text);
		}

		protected override void Creating()
		{
			
		}
	}

	public sealed class QxmlParserDefinitionThens
	{
		public IQxmlItem Item;
		public XNode Node;
	}
}
