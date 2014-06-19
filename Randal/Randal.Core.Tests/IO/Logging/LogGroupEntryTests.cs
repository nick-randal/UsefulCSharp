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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.IO.Logging;
using Randal.Core.Testing.UnitTest;
using Rhino.Mocks;

namespace Randal.Tests.Core.IO.Logging
{
	[TestClass]
	public sealed class LogGroupEntryTests : BaseUnitTest<LogGroupEntryThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveValidLogGroupEntryWhenCreating()
		{
			Given.Logger = null;
			Given.Name = "Group";
			Given.Verbosity = Verbosity.Important;
			
			When(Creating);

			Then.Entry.Should().NotBeNull()
				.And.BeAssignableTo<ILogEntry>()
				.And.BeAssignableTo<ILogGroupEntry>();
			Then.Entry.Message.Should().Be("Group");
			Then.Entry.ShowTimestamp.Should().BeFalse();
			Then.Entry.Timestamp.Should().Be(DateTime.MinValue);
			Then.Entry.IsEnd.Should().BeFalse();
			Then.Entry.VerbosityLevel.Should().Be(Verbosity.Important);
		}

		[TestMethod]
		public void ShouldAddGroupEntryMarkedAsIsEndWhenDisposingGivenLoggerInstance()
		{
			Given.Logger = MockRepository.GenerateMock<ILogger>();

			Given.Name = "GroupA";
			Given.Verbosity = Verbosity.Info;

			When(Creating, Disposing);

			Then.Logger.AssertWasCalled(x => x.Add(Arg<ILogGroupEntry>.Is.NotNull));
		}

		private void Creating()
		{
			Then.Entry = new LogGroupEntry(Given.Logger, Given.Name, Given.Verbosity);
		}

		private void Disposing()
		{
			Then.Logger = Given.Logger;
			Then.Entry.Dispose();
		}
	}

	public sealed class LogGroupEntryThens
	{
		public LogGroupEntry Entry;
		public ILogger Logger;
	}
}
