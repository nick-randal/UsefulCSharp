// Useful C#
// Copyright (C) 2014 Nicholas Randal
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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.IO.Logging;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.IO.Logging
{
	[TestClass]
	public sealed class StringLoggerTests : BaseUnitTest<StringLoggerThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestCleanup]
		public void Teardown()
		{
			if (Then.Logger != null)
				Then.Logger.Dispose();
		}

		[TestMethod]
		public void ShouldHaveValidLoggerWhenCreating()
		{
			When(Creating);

			Then.Logger.Should().NotBeNull().And.BeAssignableTo<ILogger>();
			Then.Logger.VerbosityThreshold.Should().Be(Verbosity.All);
		}

		[TestMethod]
		public void ShouldHaveSameTextWhenGettingTextGivenValue()
		{
			Given.Entry = new LogEntry("Hello world", new DateTime(2014, 6, 11));
			When(Creating, GettingText);
			Then.Text.Should().Be("140611 000000    Hello world\r\n");
		}

		[TestMethod]
		public void ShouldChangeValueWhenChangingVerbosityGivenDifferentVerbosityLevel()
		{
			Given.Verbosity = Verbosity.Vital;
			When(Creating, GettingChangedVerbosity);
			Then.Verbosity.Should().Be(Verbosity.Vital);
		}

		[TestMethod]
		public void ShouldHaveNoTextWhenGettingTextGivenPreviousTextAndLoggerIsDisposed()
		{
			Given.Entry = new LogEntry("Hello world");
			Given.ObjectIsDisposed = true;

			When(Creating, GettingText);

			Then.Text.Should().BeEmpty();
		}

		[TestMethod]
		public void ShouldHaveNoTextWhenGettingTextGivenPreviousTextAndLoggerIsDisposed2()
		{
			Given.Entry = new LogEntry("Hello world");
			Given.ObjectIsDisposed = true;

			When(Creating, GettingText, Disposing);

			Then.Text.Should().BeEmpty();
		}

		private void Creating()
		{
			Then.Logger = new StringLogger();
		}

		private void Disposing()
		{
			Then.Logger.Dispose();
		}

		private void GettingChangedVerbosity()
		{
			Then.Logger.ChangeVerbosityThreshold(Given.Verbosity);
			Then.Verbosity = Then.Logger.VerbosityThreshold;
		}

		private void GettingText()
		{
			Then.Logger.Add(Given.Entry);
			if (Given.ObjectIsDisposed != null && Given.ObjectIsDisposed == true)
				Then.Logger.Dispose();
			Then.Text = Then.Logger.GetLatestText();
		}
	}

	public sealed class StringLoggerThens
	{
		public StringLogger Logger;
		public string Text;
		public Verbosity Verbosity;
	}
}