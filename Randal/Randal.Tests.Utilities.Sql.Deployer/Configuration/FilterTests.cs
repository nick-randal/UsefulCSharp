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

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Configuration;
using Randal.Sql.Deployer.Scripts;
using System.Collections.Generic;

namespace Randal.Tests.Sql.Deployer.Configuration
{
	/// <summary>
	/// Created by nrandal on 7/21/2015 7:51:40 AM
	/// </summary>
	[TestClass]
	public sealed class FilterTests : UnitTestBase<FilterTests.Thens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidInstance_WhenCreating()
		{
			When(Creating);

			Then.Target.Should().NotBeNull();
			Then.Target.ValidationFilterConfig.WarnOn.Should().HaveCount(2);
			Then.Target.ValidationFilterConfig.HaltOn.Should().HaveCount(2);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveWarnings_WhenCheckingScript()
		{
			Given.Script = TestScript;

			When(CheckingForWarnings);

			Then.Warnings.Should().HaveCount(6);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveHaltings_WhenCheckingScript()
		{
			Given.Script = TestScript;

			When(CheckingForHaltings);

			Then.Haltings.Should().HaveCount(6);
		}

		protected override void Creating()
		{
			using (var reader = new FileInfo("TestFiles\\config.json").OpenText())
				Then.Target = JsonConvert.DeserializeObject<ScriptDeployerConfig>(reader.ReadToEnd());
		}

		private void CheckingForWarnings()
		{
			Then.WarningFilters = Then.Target.ValidationFilterConfig.WarnOn
				.Select(x => new Regex(x, ScriptChecker.RegexStandardOptions)).ToList();

			string s = Given.Script;
			Then.Warnings = Then.WarningFilters
				.SelectMany(x => x.Matches(s).OfType<Match>().Select(y => y).ToArray())
				.Where(m => m.Success)
				.ToList();
		}

		private void CheckingForHaltings()
		{
			Then.HaltingFilters = Then.Target.ValidationFilterConfig.HaltOn
				.Select(x => new Regex(x, ScriptChecker.RegexStandardOptions)).ToList();

			string s = Given.Script;
			Then.Haltings = Then.HaltingFilters
				.SelectMany(x => x.Matches(s).OfType<Match>().Select(y => y).ToArray())
				.Where(m => m.Success)
				.ToList();
		}

		public sealed class Thens
		{
			public ScriptDeployerConfig Target;
			public List<Regex> WarningFilters;
			public List<Regex> HaltingFilters;
			public List<Match> Haltings;
			public List<Match> Warnings;
		}

		private const string TestScript = @"

-- Warning

CREATE TABLE Patient
CREATE VIEW vPatient
CREATE PROCEDURE sp_TestCreate
CREATE FUNCTION GetPatient
CREATE INDEX IX_Patient_ID

EXEC('test')

-- Halt

DROP TABLE Patient
DROP VIEW vPatient
DROP PROCEDURE sp_TestCreate
DROP FUNCTION GetPatient
DROP INDEX IX_Patient_ID

TRUNCATE TABLE Patient

-- OK

EXEC sp_TestCreate()
CREATE TABLE #TempData
DROP TABLE #TempData
CREATE TYPE tvpPatient
DROP TYPE tvpPatient
";
	}
}
