using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.QuickXml;

namespace Randal.Tests.QuickXml
{
	[TestClass]
	public sealed class QAttributeTests : BaseUnitTest<QAttributeThens>
	{
		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenConvertingToNode()
		{
			Given.Depth = 0;
			Given.Name = "Age";
			Given.Value = "32";

			ThrowsExceptionWhen(ConvertingToNode);

			ThenLastAction.ShouldThrow<NotSupportedException>();
		}

		protected override void Creating()
		{
			Then.Target = new QAttribute(Given.Depth, Given.Name, Given.Value);
		}

		private void ConvertingToNode()
		{
			Then.Target.ToNode();
		}
	}

	public sealed class QAttributeThens
	{
		public QAttribute Target;
	}
}
