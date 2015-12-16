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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;

namespace Randal.Tests.Logging
{
	[TestClass]
	public sealed class NullLoggerTests : UnitTestBase<NullLoggerThens>
	{
		protected override void OnTeardown()
		{
			if (Then.Log != null)
				Then.Log.Dispose();
		}

		[TestMethod]
		public void ShouldHaveValidNullLoggerWhenCreating()
		{
			When(Creating);

			Then.Log.Should().NotBeNull().And.BeAssignableTo<ILogSink>();
			Then.Log.VerbosityThreshold.Should().Be(Verbosity.All);
		}

		[TestMethod]
		public void ShouldNotThrowExceptionWhenAddingAnEntry()
		{
			Given.Entry = new LogEntry("Test");

			When(AddingLogEntry);
		}

		[TestMethod]
		public void ShouldHaveUnchangedVerbosityLevelWhenChangingVerbosity()
		{
			Given.Verbosity = Verbosity.Vital;
			When(ChangingVerbosity);
			Then.Log.VerbosityThreshold.Should().Be(Verbosity.All);
		}

		private void ChangingVerbosity()
		{
			Then.Log.ChangeVerbosityThreshold(Given.Verbosity);
		}

		private void AddingLogEntry()
		{
			Then.Log.Post(Given.Entry);
		}

		protected override void Creating()
		{
			Then.Log = new NullLogSink();
		}
	}

	public sealed class NullLoggerThens
	{
		public NullLogSink Log;
	}
}