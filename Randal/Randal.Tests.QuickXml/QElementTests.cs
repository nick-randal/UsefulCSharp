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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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