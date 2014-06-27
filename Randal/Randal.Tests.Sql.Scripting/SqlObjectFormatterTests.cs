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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Scripting;
using FluentAssertions;
using Microsoft.SqlServer.Management.Smo;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class SqlObjectFormatterTests : BaseUnitTest<SqlObjectFormatterThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveUsableFormatterWhenCreating()
		{
			When(Creating);
			Then.Formatter.Should().NotBeNull().And.BeAssignableTo<ISqlObjectFormatter>();
		}

		[TestMethod]
		public void ShouldHaveScriptWhenFormattingObjectGivenStoredProcedure()
		{
			Given.Object = new Server(".").Databases["master"].StoredProcedures.Cast<StoredProcedure>().First();
			When(Creating, Formatting);
			Then.Text.Should().StartWith("--:: catalog master").And.Contain("--:: main");
		}

		private void Creating()
		{
			Then.Formatter = new SqlObjectFormatter();
		}

		private void Formatting()
		{
			Then.Text = Then.Formatter.Format(Given.Object);
		}
	}

	public class SqlObjectFormatterThens
	{
		public SqlObjectFormatter Formatter;
		public string Text;
	}
}