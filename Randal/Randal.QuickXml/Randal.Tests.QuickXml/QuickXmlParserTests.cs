using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using FluentAssertions;
using Randal.QuickXml;
using System.Xml.Linq;

namespace Randal.Tests.QuickXml
{
	[TestClass]
	public sealed class QuickXmlParserTests : BaseUnitTest<QuickXmlParserThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidInstance_WhenCreating_GivenDefaults()
		{
			When(Creating);

			Then.Target.Should().NotBeNull().And.Should().BeAssignableTo<IQuickXmlParser>();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveXmlDocument_WhenParsing_GivenDocumentStructure()
		{
			Given.Text = "A\n\tB";

			When(Parsing);

			Then.Document.Should().NotBeNull();
			Then.Document.Root.Name.Should().Be("A");
		}

		private void Parsing()
		{
			throw new NotImplementedException();
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

	}
}
