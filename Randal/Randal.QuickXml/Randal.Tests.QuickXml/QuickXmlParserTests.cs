using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using FluentAssertions;
using Randal.QuickXml;
using System.Xml.Linq;
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
		public void ShouldHaveXmlDocument_WhenParsing_GivenDocumentStructure()
		{
			Given.Text = "A\n\tB";

			When(Parsing);

			Then.Fragment.Should().NotBeNull();
			Then.Fragment.Name.LocalName.Should().Be("A");
			Then.Fragment.Elements().First().Name.LocalName.Should().Be("B");
		}

		private void Parsing()
		{
			Then.Fragment = Then.Target.ParseToXElement(new StringReader(Given.Text));
		}

		protected override void Creating()
		{
			Then.Target = new QuickXmlParser();
		}
	}

	public sealed class QuickXmlParserThens
	{
		public QuickXmlParser Target;
		public XDocument Document;
		public XElement Fragment;
	}
}
