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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Configuration;

namespace Randal.Tests.Sql.Deployer.Configuration
{
	[TestClass]
	public sealed class InstallerConfigTests : BaseUnitTest<InstallerConfigThens>
	{
		[TestMethod]
		public void ShouldHaveDefaultConfigWhenCreating()
		{
			Given.Json = "{}";

			When(Creating);

			Then.Object.Should().NotBeNull();
			Then.Object.PriorityScripts.Should().HaveCount(0);
			Then.Object.Project.Should().Be("Unknown");
			Then.Object.Version.Should().Be("01.01.01.01");
		}

		[TestMethod]
		public void ShouldHaveValidValuesWhenCreatingGivenValueJson()
		{
			Given.Json = "{ Version: '14.06.04.01', Project: 'UnitTest', PriorityScripts: [ 'A' ] }";

			When(Creating);

			Then.Object.PriorityScripts.Should().HaveCount(1);
			Then.Object.PriorityScripts.FirstOrDefault().Should().Be("A");
			Then.Object.Version.Should().Be("14.06.04.01");
			Then.Object.Project.Should().Be("UnitTest");
		}

		private void Creating()
		{
			Then.Object = JsonConvert.DeserializeObject<ProjectConfig>(Given.Json);
		}
	}

	public sealed class InstallerConfigThens
	{
		public IProjectConfig Object;
	}
}