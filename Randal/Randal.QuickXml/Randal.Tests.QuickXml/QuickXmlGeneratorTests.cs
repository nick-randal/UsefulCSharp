using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.QuickXml;

namespace Randal.Tests.QuickXml
{
	[TestClass]
	public sealed class QuickXmlGeneratorTests : BaseUnitTest<QuickXmlGeneratorThens>
	{
		[TestMethod, PositiveTest, DeploymentItem("Test Files\\", "Test Files\\")]
		public void ShouldHaveQuickXml_WhenGenerating_GivenXml()
		{
			Given.Xml = XElement.Parse(File.ReadAllText("Test Files\\B.xml"));

			When(Generating);

			var quickXml = File.ReadAllText("Test Files\\B.qxml").NoWhitespace();
			Then.QXml.NoWhitespace().Should().Be(quickXml);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValidText_WhenGenerating_GivenElementWithText()
		{
			Given.Xml = new XElement("Test", new XText("Hello world"));

			When(Generating);

			Then.QXml.Should().Be("Test\r\n\t\"Hello world\"\r\n");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValidText_WhenGenerating_GivenElementWithCData()
		{
			Given.Xml = new XElement("Test", new XCData("Hello world"));

			When(Generating);

			Then.QXml.Should().Be("Test\r\n\t[Hello world]\r\n");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValidText_WhenGenerating_GivenDocument()
		{
			Given.Xml = new XDocument(new XElement("Root"));

			When(Generating);

			Then.QXml.Should().Be("Root\r\n");
		}

		private void Generating()
		{
			using (var writer = new StringWriter())
			{
				Then.Target.GenerateQuickXml(writer, Given.Xml);
				writer.Flush();
				Then.QXml = writer.ToString();
			}
		}

		protected override void Creating()
		{
			Then.Target = new QuickXmlGenerator();
		}
	}

	public sealed class QuickXmlGeneratorThens
	{
		public QuickXmlGenerator Target;
		public string QXml;
	}
}
