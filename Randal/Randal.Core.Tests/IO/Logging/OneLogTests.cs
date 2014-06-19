/*
Useful C#
Copyright (C) 2014  Nicholas Randal

Useful C# is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.IO.Logging;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.IO.Logging
{
	[TestClass]
	public sealed class OneLogTests : BaseUnitTest<object>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveStringLoggerWhenAccessingInstance()
		{
			OneLog.Inst.Should().NotBeNull().And.BeAssignableTo<StringLogger>();
		}
	}
}
