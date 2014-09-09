using System.Diagnostics;
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

			Then.Fragment.Should().NotBeNull();
			Then.Fragment.Name.LocalName.Should().Be("A");
			Then.Fragment.Elements().First().Name.LocalName.Should().Be("B");
		}

		[TestMethod, PositiveTest, DeploymentItem("Test Files\\A.qxml", "Test Files")]
		public void ShouldHaveXmlDocument_WhenParsing_GivenFullDocument()
		{
			Given.Text = File.ReadAllText("Test Files\\A.qxml");

			When(Parsing);

			Then.Fragment.Should().NotBeNull();
			
			var element = Then.Fragment.XPathSelectElement("./Departments/Department[2]/People/Person[1]");
			element.Should().NotBeNull().And.BeOfType<XElement>();
			element.Value.Should().Be("Buffalo Bill");
			element.Attribute("Age").Value.Should().Be("165");
		}

		[TestMethod, PositiveTest, DeploymentItem("Test Files\\A.qxml", "Test Files")]
		public void ShouldHaveElementWithCData_WhenParsing_GivenFullDocument()
		{
			Given.Text = File.ReadAllText("Test Files\\A.qxml");

			When(Parsing);

			Then.Fragment.Should().NotBeNull();

			var element = Then.Fragment.XPathSelectElement("./Data");
			element.Should().NotBeNull().And.BeOfType<XElement>();
			element.Value.Should().Be("Some data here");
		}

		[TestMethod, PositiveTest, DeploymentItem("Test Files\\A.qxml", "Test Files")]
		public void ShouldHaveComment_WhenParsing_GivenFullDocument()
		{
			Given.Text = File.ReadAllText("Test Files\\A.qxml");

			When(Parsing);

			Then.Fragment.Should().NotBeNull();

			var comment = Then.Fragment.DescendantNodes().FirstOrDefault(node => node.NodeType == XmlNodeType.Comment) as XComment;
			comment.Should().NotBeNull().And.BeAssignableTo<XComment>();
			comment.Value.Should().Be("No comment!");
			comment.Parent.Name.LocalName.Should().Be("Departments");
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
			Then.Fragment = Then.Target.ParseToXElement(new StringReader(Given.Text));
		}

		protected override void Creating()
		{
			Then.Target = new QuickXmlParser(QuickXmlParserDefinition.QxmlItems);
		}
	}

	public sealed class QuickXmlParserThens
	{
		public QuickXmlParser Target;
		public XDocument Document;
		public XElement Fragment;
	}
}
