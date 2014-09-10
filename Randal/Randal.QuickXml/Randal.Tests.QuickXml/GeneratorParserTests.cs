// // Useful C#
// // Copyright (C) 2014 Nicholas Randal
// // 
// // Useful C# is free software; you can redistribute it and/or modify
// // it under the terms of the GNU General Public License as published by
// // the Free Software Foundation; either version 2 of the License, or
// // (at your option) any later version.
// // 
// // This program is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// // GNU General Public License for more details.

using System.IO;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.QuickXml;

namespace Randal.Tests.QuickXml
{
	[TestClass]
	public sealed class GeneratorParserTests : BaseUnitTest<GeneratorParserThens>
	{
		[TestMethod, PositiveTest, DeploymentItem("Test Files\\B.xml", "Test Files\\")]
		public void ShouldHaveSameXml_WhenGeneratingAndParsingXElement_GivenValidXml()
		{
			Given.Xml = XElement.Parse(File.ReadAllText("Test Files\\B.xml"));

			When(Generating, ThenParsingXElement);

			XNode.DeepEquals((XElement) Given.Xml, Then.Element)
				.Should()
				.BeTrue(" Quick Xml was generated from an xml file and then parsed back to Xml");
		}

		[TestMethod, PositiveTest, DeploymentItem("Test Files\\C.xml", "Test Files\\")]
		public void ShouldHaveSameXml_WhenGeneratingAndParsingXDocument_GivenValidXml()
		{
			Given.Xml = XDocument.Parse(File.ReadAllText("Test Files\\C.xml"));

			When(Generating, ThenParsingXDocument);

			var doc = (XDocument) Given.Xml;

			doc.ToString()
				.Should()
				.BeEquivalentTo(Then.Document.ToString(),
					" Quick Xml was generated from an xml file and then parsed back to Xml but '{0}' does not match '{1}'", Given.Xml,
					Then.Document);
		}

		protected override void Creating()
		{
			Then.Generator = new QuickXmlGenerator();
			Then.Parser = new QuickXmlParser();
		}

		private void Generating()
		{
			using (var writer = new StringWriter())
			{
				Then.Generator.GenerateQuickXml(writer, Given.Xml);
				Then.QuickXml = writer.ToString();
			}
		}

		private void ThenParsingXElement()
		{
			Then.Element = Then.Parser.ParseToXElement(Then.QuickXml);
		}

		private void ThenParsingXDocument()
		{
			Then.Document = Then.Parser.ParseToXDocument(Then.QuickXml);
		}
	}

	public sealed class GeneratorParserThens
	{
		public QuickXmlGenerator Generator;
		public QuickXmlParser Parser;
		public string QuickXml;
		public XElement Element;
		public XDocument Document;
	}
}