using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.QuickXml;
using Randal.Core.Testing.UnitTest;
using FluentAssertions;
using System.Xml.Linq;

namespace Randal.Tests.QuickXml
{
	[TestClass]
	public sealed class QDataTests : BaseUnitTest<QDataThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidInstance_WhenCreating_GivenValues()
		{
			Given.Depth = 3;
			Given.Value = "What about bob?";

			When(Creating);

			Then.Target.Should().NotBeNull();
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenGettingName()
		{
			Given.Depth = 0;
			Given.Value = string.Empty;

			ThrowsExceptionWhen(GettingName);

			ThenLastAction.ShouldThrow<NotSupportedException>();
		}

		protected override void Creating()
		{
			Then.Target = new QData(Given.Depth, Given.Value);
		}

		private void GettingName()
		{
			var name = Then.Target.Name;
		}
	}

	public sealed class QDataThens
	{
		public QData Target;
		public XNode Node;
	}
}
