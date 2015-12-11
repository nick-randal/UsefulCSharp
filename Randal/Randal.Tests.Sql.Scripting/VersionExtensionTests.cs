// Useful C#
// Copyright (C) 2014-2016 Nicholas Randal
// 
// Useful C# is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

using System;
using System.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Scripting;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class VersionExtensionTests : UnitTestBase<VersionExtensionThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidVersion_WhenConverting_GivenIterationNumber()
		{
			Given.Iteration = 5;
			Given.Today = new DateTime(2014, 8, 11);

			When(ConvertingFromIteration);

			Then.Version.Should().Be("14.08.11.05");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValidVersion_WhenConverting_GivenDate()
		{
			Given.Iteration = 5;
			Given.Date = new DateTime(2014, 8, 11);

			When(ConvertingFromDate);

			Then.Version.Should().Be("14.08.11.05");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveCappedIteration_WhenConverting_GivenIterationUnderLimit()
		{
			Given.Iteration = -1;
			Given.Date = new DateTime(2014, 8, 11);

			When(ConvertingFromDate);

			Then.Version.Should().Be("14.08.11.01");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveCappedIteration_WhenConverting_GivenIterationOverLimit()
		{
			Given.Iteration = 100;
			Given.Date = new DateTime(2014, 8, 11);

			When(ConvertingFromDate);

			Then.Version.Should().Be("14.08.11.99");
		}

		private void ConvertingFromIteration()
		{
			using (ShimsContext.Create())
			{
				ShimDateTime.TodayGet = () => Given.Today;
				Then.Version = ((int)Given.Iteration).ToVersionToday();
			}
		}

		private void ConvertingFromDate()
		{
			Then.Version = ((DateTime)Given.Date).ToVersion((int)Given.Iteration);
		}

		protected override void Creating()
		{
		}
	}

	public sealed class VersionExtensionThens
	{
		public string Version;
	}
}
