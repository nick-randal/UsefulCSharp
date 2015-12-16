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
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Scripts;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.Scripts.Blocks
{
	[TestClass]
	public sealed class SqlCommandBlockTests : UnitTestBase<SqlCommandBlockThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveInstanceAfterConstruction()
		{
			Given.Keyword = "pre";
			Given.Text = "\nselect 1\ngo";
			Given.Phase = SqlScriptPhase.Pre;

			When(Creating);

			Then.Object.Keyword.Should().Be("pre");
			Then.Object.IsExecuted.Should().BeFalse();
			Then.Object.Text.Should().NotBeNullOrWhiteSpace();
			Then.Object.Phase.Should().Be(SqlScriptPhase.Pre);
		}

		[TestMethod, PositiveTest]
		public void ShouldParseValidSqlTextBlock()
		{
			Given.Text = "\nselect 1\n\t go \t\r\nselect ' go '\ngo";
			Given.Keyword = "post";
			Given.Phase = SqlScriptPhase.Post;

			When(ParsingSqlTextBlock);

			Then.Object.IsValid.Should().BeTrue();
			Then.Object.IsExecuted.Should().BeFalse();
			Then.Object.Phase.Should().Be(SqlScriptPhase.Post);
			Then.Messages.Should().HaveCount(0);
		}

		[TestMethod, PositiveTest]
		public void ShouldNotHaveNullPropertiesWhenInitializedWithNullValues()
		{
			Given.Text = null;
			Given.Keyword = null;
			Given.Phase = SqlScriptPhase.Pre;

			When(Creating);

			Then.Object.Text.Should().NotBeNull().And.BeEmpty();
			Then.Object.Keyword.Should().NotBeNull().And.BeEmpty();
		}

		[TestMethod, PositiveTest]
		public void ShouldReturnTextAndMarkedAsExecuted_WhenRequestingForExecution()
		{
			Given.Text = "select 1";
			Given.Keyword = "main";
			Given.Phase = SqlScriptPhase.Main;

			When(ParsingSqlTextBlock, RequestingForExecution);

			Then.CommandText.Should().Be("select 1");
			Then.Object.IsExecuted.Should().BeTrue();
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenRequestingExecutionMoreThanOnce()
		{
			Given.Text = "Select 1";
			Given.Keyword = "main";
			Given.Phase = SqlScriptPhase.Main;

			WhenLastActionDeferred(ParsingSqlTextBlock, RequestingForExecution, RequestingForExecution);

			ThenLastAction.ShouldThrow<InvalidOperationException>().WithMessage("Cannot request execution for script block more than once.");
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenRequestingExecution_GivenUnparsedBlock()
		{
			Given.Text = "Select 1";
			Given.Keyword = "main";
			Given.Phase = SqlScriptPhase.Main;

			WhenLastActionDeferred(RequestingForExecution);

			ThenLastAction.ShouldThrow<InvalidOperationException>().WithMessage("Cannot execute invalid script block.");
		}

		protected override void Creating()
		{
			Then.Object = new SqlCommandBlock(Given.Keyword, Given.Text, Given.Phase);
		}

		private void ParsingSqlTextBlock()
		{
			Then.Messages = Then.Object.Parse();
		}

		private void RequestingForExecution()
		{
			Then.CommandText = Then.Object.RequestForExecution();
		}
	}

	public sealed class SqlCommandBlockThens
	{
		public SqlCommandBlock Object;
		public IReadOnlyList<string> Messages;
		public string CommandText;
	}
}