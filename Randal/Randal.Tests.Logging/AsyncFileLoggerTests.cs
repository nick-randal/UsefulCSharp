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
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;

namespace Randal.Tests.Logging
{
	[TestClass]
	public sealed class AsyncFileLoggerTests : BaseUnitTest<AsyncFileLoggerThens>
	{
		protected override void OnSetup()
		{
		}

		[TestMethod]
		public void ShouldHaveFileLoggerWhenCreating()
		{
			When(Creating);

			Then.Logger.Should().NotBeNull().And.BeAssignableTo<ILogger>();
			Then.Logger.VerbosityThreshold.Should().Be(Verbosity.All);
			Then.Logger.State.Should().Be(AsyncFileLoggerState.Running);
		}

		[TestMethod]
		public void ShouldChangeValueWhenSettingVerbosityGivenNewVerbosityLevel()
		{
			Given.Verbosity = Verbosity.Important;

			When(SettingVerbosity);

			Then.Logger.VerbosityThreshold.Should().Be(Verbosity.Important);
		}

		[TestMethod, ExpectedException(typeof (ArgumentNullException))]
		public void ShouldThrowExceptionWhenCreatingGivenNullSettings()
		{
			Given.NullSettings = true;

			When(Creating);
		}

		private void Disposing()
		{
			Then.Logger.Dispose();
		}

		private void SettingVerbosity()
		{
			Then.Logger.ChangeVerbosityThreshold(Given.Verbosity);
		}

		protected override void Creating()
		{
			var settings = new FileLoggerSettings(Test.Paths.LoggingFolder, "Test", 1024);

			if(GivensDefined("NullSettings") && Given.NullSettings == true)
				Then.Logger = new AsyncFileLogger(null);
			else
				Then.Logger = new AsyncFileLogger(settings);
			Thread.Sleep(100);
		}
	}

	public sealed class AsyncFileLoggerThens
	{
		public AsyncFileLogger Logger;
	}
}