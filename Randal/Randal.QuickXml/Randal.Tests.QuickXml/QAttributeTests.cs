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
		protected override void Creating()
		{
			Then.Target = new QAttribute(Given.Depth, Given.Name, Given.Value);
		}
	}

	public sealed class QAttributeThens
	{
		public QAttribute Target;
	}
}
