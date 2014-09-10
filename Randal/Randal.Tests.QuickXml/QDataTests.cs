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

using System;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.QuickXml;

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