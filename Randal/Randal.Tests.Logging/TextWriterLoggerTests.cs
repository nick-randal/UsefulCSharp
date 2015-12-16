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
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;
using Rhino.Mocks;

namespace Randal.Tests.Logging
{
	[TestClass]
	public sealed class TextWriterLoggerTests : UnitTestBase<TextWriterLoggerThens>
	{
		[TestMethod]
		public void ShouldHaveTextWriterLoggerWhenCreatingInstanceGivenConsoleOut()
		{
			Given.Stream = Console.Out;
			When(Creating);
			Then.Logger.Should().NotBeNull().And.BeAssignableTo<ILogSink>();
		}

		[TestMethod]
		public void ShouldCallWriteLineWhenWritingGivenEntry()
		{
			Given.Entry = new LogEntry("Test");
			When(Writing);
			Then.Writer.AssertWasCalled(x => x.Write(Arg<string>.Is.NotNull));
		}

		protected override void Creating()
		{
			Then.Writer = Given.Stream ?? MockRepository.GenerateMock<TextWriter>();
			Then.Logger = new TextWriterLogSink(Then.Writer);
		}

		private void Writing()
		{
			Then.Logger.Post(Given.Entry);
		}
	}

	public sealed class TextWriterLoggerThens
	{
		public TextWriterLogSink Logger;
		public TextWriter Writer;
	}
}