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
		[TestMethod, PositiveTest, DeploymentItem("Test Files\\B.xml", "Test Files")]
		public void ShouldHaveQuickXml_WhenGenerating_GivenXml()
		{
			Given.Text = File.ReadAllText("Test Files\\B.xml");

			When(Generating);

			Then.QXml.Should().Be("");
		}

		private void Generating()
		{
			var root = XElement.Parse(Given.Text);
			using (var writer = new StringWriter())
			{
				Then.Target.GenerateQXml(writer, root);
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
