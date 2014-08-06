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
	public sealed class VersionExtensionTests : BaseUnitTest<VersionExtensionThens>
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
