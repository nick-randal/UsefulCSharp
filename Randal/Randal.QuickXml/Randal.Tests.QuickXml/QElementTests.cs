using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.QuickXml;

namespace Randal.Tests.QuickXml
{
	[TestClass]
	public sealed class QElementTests : BaseUnitTest<QElementThens>
	{
		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenAccessingName_GivenElement()
		{
			ThrowsExceptionWhen(() => { var value = Then.Target.Value; });

			ThenLastAction.ShouldThrow<NotSupportedException>();
		}

		protected override void Creating()
		{
			Then.Target = new QElement(0, "Test");
		}
	}

	public sealed class QElementThens
	{
		public QElement Target;
	}
}
