// Useful C#
// Copyright (C) 2014-2015 Nicholas Randal
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
using System.Fakes;
using System.Linq;
using FluentAssertions;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;

namespace Randal.Core.Testing.Factory.Tests
{
	/// <summary>
	/// Created by nrandal on 9/10/2015 10:24:57 AM
	/// </summary>
	[TestClass]
	public sealed class AutoModelFactoryTests : BaseUnitTest<AutoModelFactoryTests.Thens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidInstance_WhenCreating()
		{
			When(Creating);

			Then.Target.Should().NotBeNull();
		}

		[TestMethod, PositiveTest]
		public void ShouldNotThrowException_WhenPreparing()
		{
			DeferLastActionWhen(Preparing);

			ThenLastAction.ShouldNotThrow();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveModelWithValues_WhenCreatingModel_GivenGenericIncrementingValues()
		{
			When(Preparing, CreatingModel);

			Then.Model.ShouldBeEquivalentTo(new
			{
				PrimaryId = 2, Age = 1,
				FirstName = "value1", LastName = "value2",
				SecondaryId = 2L, OtherId = 1L,
				IsSomething = false, WasSomething = true,
				MiddleInitial = 'A', Status = 'B',
				Flags1 = 1, Flags2 = 2,
				CreatedOn = new DateTime(2000, 1, 1, 1, 0, 0), ChangedOn = new DateTime(2000, 1, 1),
				SquareFeet = 2, Acreage = 1,
				Salary = 2m, Bonus = 1m,
				Longitude = 2f, Latitude = 1f,
				TempId1 = 3, TempId2 = 4
			});
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveListOfModels_WhenCreatingModels_GivenGenericIncrementingValues()
		{
			Given.HowMany = 5001;

			When(Preparing, CreatingModels);

			Then.Models.Should().HaveCount(5001);
			Then.Models.Last().ShouldBeEquivalentTo(new
			{
				PrimaryId = 20002, Age = 20001,
				FirstName = "value10001", LastName = "value10002",
				SecondaryId = 10002L, OtherId = 10001L,
				IsSomething = false, WasSomething = true,
				MiddleInitial = 'Q', Status = 'R',
				Flags1 = 17, Flags2 = 18,
				CreatedOn = new DateTime(2001, 2, 20, 17, 0, 0), ChangedOn = new DateTime(2001, 2, 20, 16, 0, 0),
				SquareFeet = 10002, Acreage = 10001,
				Salary = 10002m, Bonus = 10001m,
				Longitude = 10002f, Latitude = 10001f,
				TempId1 = 20003, TempId2 = 20004
			});
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenCreatingModel_GivenDidNotCallPrepare()
		{
			DeferLastActionWhen(CreatingModel);

			ThenLastAction.ShouldThrow<InvalidOperationException>();
		}

		protected override void Creating()
		{
			using (ShimsContext.Create())
			{
				ShimDateTime.TodayGet = () => new DateTime(2000, 1, 1);
				Then.Target = new AutoModelFactory<TestModel>(Given.Values as IValueFactory);
			}
		}

		private void Preparing()
		{
			Then.Target.Prepare();
		}

		private void CreatingModel()
		{
			Then.Model = Then.Target.Create();
		}

		private void CreatingModels()
		{
			Then.Models = Then.Target.Create((int)Given.HowMany);
		}

		public sealed class Thens
		{
			public AutoModelFactory<TestModel> Target;
			public TestModel Model;
			public IEnumerable<TestModel> Models;
		}
	}
}