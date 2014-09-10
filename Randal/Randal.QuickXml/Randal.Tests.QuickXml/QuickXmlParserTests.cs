using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using FluentAssertions;
using Randal.QuickXml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;

namespace Randal.Tests.QuickXml
{
	[TestClass]
	public sealed class QuickXmlParserTests : BaseUnitTest<QuickXmlParserThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidInstance_WhenCreating_GivenDefaults()
		{
			When(Creating);

			Then.Target.Should().NotBeNull().And.BeAssignableTo<IQuickXmlParser>(" because object is a QuickXmlParser.");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveXmlDocument_WhenParsing_GivenBasicElements()
		{
			Given.Text = "A\r\n\tB";

			When(Parsing);

			Then.Xml.Name.Should().Be("A");
			Then.Xml.MoveToFirstChild();
			Then.Xml.Name.Should().Be("B");
		}

		[TestMethod, PositiveTest, DeploymentItem("Test Files\\A.qxml", "Test Files")]
		public void ShouldHaveXmlDocument_WhenParsing_GivenFullDocument()
		{
			Given.Text = File.ReadAllText("Test Files\\A.qxml");

			When(Parsing);
			
			var iterator = Then.Xml.Select("./Departments/Department[2]/People/Person[1]");
			iterator.MoveNext();
			iterator.Current.NodeType.Should().Be(XPathNodeType.Element);
			iterator.Current.Value.Should().Be("Buffalo Bill");

			iterator.Current.MoveToFirstAttribute();
			iterator.Current.Name.Should().Be("Age");
			iterator.Current.Value.Should().Be("165");
		}

		[TestMethod, PositiveTest, DeploymentItem("Test Files\\A.qxml", "Test Files")]
		public void ShouldHaveElementWithCData_WhenParsing_GivenFullDocument()
		{
			Given.Text = File.ReadAllText("Test Files\\A.qxml");

			When(Parsing);

			var iterator = Then.Xml.Select("//Data/text()");
			iterator.MoveNext();
			iterator.Current.Value.Should().Be("Some data here");
		}

		[TestMethod, PositiveTest, DeploymentItem("Test Files\\A.qxml", "Test Files")]
		public void ShouldHaveComment_WhenParsing_GivenFullDocument()
		{
			Given.Text = File.ReadAllText("Test Files\\A.qxml");

			When(Parsing);

			Then.Xml.MoveToFirstChild();
			Then.Xml.MoveToChild(XPathNodeType.Comment).Should().BeTrue();
			Then.Xml.NodeType.Should().Be(XPathNodeType.Comment);
			Then.Xml.Value.Should().Be("No comment!");

			Then.Xml.MoveToParent();
			Then.Xml.Name.Should().Be("Departments");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveXElements_WhenParsing_GivenQualifiedElements()
		{
			Given.Text = "a\r\nxmlns:n http://test.com\r\nn:b c\r\n\tn:sub";

			When(Parsing);

			Then.Xml.Name.Should().Be("a");
			Then.Xml.MoveToFirstAttribute();
			Then.Xml.Name.Should().Be("n:b");
			Then.Xml.Value.Should().Be("c");

			Then.Xml.MoveToParent();
			Then.Xml.MoveToFirstChild();
			Then.Xml.Name.Should().Be("n:sub");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveXDocument_WhenParsing_GivenQualifiedElements()
		{
			Given.Text = "a\r\n\tb\r\n\tc d";

			When(ParsingToDocument);

			Then.Xml.MoveToFirstChild();
			Then.Xml.Name.Should().Be("a");

			Then.Xml.MoveToFirstChild();
			Then.Xml.Name.Should().Be("b");
			Then.Xml.MoveToFirstAttribute();
			Then.Xml.Name.Should().Be("c");
			Then.Xml.Value.Should().Be("d");
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenParsing_GivenElementsAtRootDepth()
		{
			Given.Text = "A\r\nB";

			ThrowsExceptionWhen(Parsing);

			ThenLastAction.ShouldThrow<InvalidDataException>();
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenParsing_GivenFirstItemIsNotAnElement()
		{
			Given.Text = "!A";

			ThrowsExceptionWhen(Parsing);

			ThenLastAction.ShouldThrow<InvalidDataException>();
		}

		private void Parsing()
		{
			using (var reader = new StringReader(Given.Text))
				Then.Xml = Then.Target.ParseToXElement(reader).CreateNavigator();
		}

		private void ParsingToDocument()
		{
			using (var reader = new StringReader(Given.Text))
				Then.Xml = Then.Target.ParseToXDocument(reader).CreateNavigator();
		}

		protected override void Creating()
		{
			Then.Target = new QuickXmlParser(QuickXmlParserDefinition.QxmlItems);
		}
	}

	public sealed class QuickXmlParserThens
	{
		public QuickXmlParser Target;
		public XPathNavigator Xml;
	}
}
