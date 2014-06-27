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
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Scripts;

namespace Randal.Tests.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class SqlCommandBlockTests : BaseUnitTest<SqlCommandBlockThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
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

		[TestMethod]
		public void ShouldParseValidSqlTextBlock()
		{
			Given.Text = "\nselect 1\n\t go \t\r\nselect ' go '\ngo";
			Given.Keyword = "post";
			Given.Phase = SqlScriptPhase.Post;

			When(Creating, ParsingSqlTextBlock);

			Then.Object.IsValid.Should().BeTrue();
			Then.Object.IsExecuted.Should().BeFalse();
			Then.Object.Phase.Should().Be(SqlScriptPhase.Post);
			Then.Messages.Should().HaveCount(0);
		}

		[TestMethod]
		public void ShouldNotHaveNullPropertiesWhenInitializedWithNullValues()
		{
			Given.Text = null;
			Given.Keyword = null;
			Given.Phase = SqlScriptPhase.Pre;

			When(Creating);

			Then.Object.Text.Should().NotBeNull().And.BeEmpty();
			Then.Object.Keyword.Should().NotBeNull().And.BeEmpty();
		}

		[TestMethod]
		public void ShouldReturnTextAndMarkedAsExecutedWhenRequestingForExecution()
		{
			Given.Text = "select 1";
			Given.Keyword = "main";
			Given.Phase = SqlScriptPhase.Main;

			When(Creating, ParsingSqlTextBlock, RequestingForExecution);

			Then.CommandText.Should().Be("select 1");
			Then.Object.IsExecuted.Should().BeTrue();
		}

		[TestMethod, ExpectedException(typeof (InvalidOperationException))]
		public void ShouldThrowExceptionWhenRequestingExecutionMoreThanOnce()
		{
			Given.Text = "Select 1";
			Given.Keyword = "main";
			Given.Phase = SqlScriptPhase.Main;

			When(Creating, RequestingForExecution, RequestingForExecution);
		}

		[TestMethod,
		 ExpectedException(typeof (InvalidOperationException), "Cannot execute invalid script block.")]
		public void ShouldThrowExceptionWhenRequestingExecutionGivenUnparsedBlock()
		{
			Given.Text = "Select 1";
			Given.Keyword = "main";
			Given.Phase = SqlScriptPhase.Main;

			When(Creating, RequestingForExecution);
		}

		private void Creating()
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