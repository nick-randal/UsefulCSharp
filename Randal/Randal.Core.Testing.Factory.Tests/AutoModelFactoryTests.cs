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
	public sealed class AutoModelFactoryTests : UnitTestBase<AutoModelFactoryTests.Thens>
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
			WhenLastActionDeferred(Preparing);

			ThenLastAction.ShouldNotThrow();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveModelWithValues_WhenCreatingModel_GivenGenericIncrementingValues()
		{
			When(Preparing, CreatingModel);

			Then.Model.ShouldBeEquivalentTo(new
			{
				PrimaryId = 1, Age = 1,
				FirstName = "FirstName1", LastName = "LastName1",
				SecondaryId = 1L, OtherId = 1L,
				IsSomething = true, WasSomething = true,
				MiddleInitial = 'A', Status = 'A',
				Flags1 = 1, Flags2 = 1,
				CreatedOn = new DateTime(2000, 1, 1), ChangedOn = new DateTime(2000, 1, 1),
				SquareFeet = 1, Acreage = 1,
				Salary = 1m, Bonus = 1m,
				Longitude = 1f, Latitude = 1f,
				TempId1 = 1, TempId2 = 1,
				BigPrecision = 1d,
				ChildObject = (object)null,
				ChildModel1 = (OtherModel)null,
				ChildModel2 = (OtherModel)null
			});
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveListOfModels_WhenCreatingModels_GivenGenericIncrementingValues()
		{
			Given.HowMany = 1234;

			When(Preparing, CreatingModels);

			Then.Models.Should().HaveCount(1234);
			Then.Models.Last().ShouldBeEquivalentTo(new
			{
				PrimaryId = 1234, Age = 1234,
				FirstName = "FirstName1234", LastName = "LastName1234",
				SecondaryId = 1234L, OtherId = 1234L,
				IsSomething = false, WasSomething = false,
				MiddleInitial = 'L', Status = 'L',
				Flags1 = 210, Flags2 = 210,
				CreatedOn = new DateTime(2000, 2, 21, 9, 0, 0), ChangedOn = new DateTime(2000, 2, 21, 9, 0, 0),
				SquareFeet = 1234, Acreage = 1234,
				Salary = 1234m, Bonus = 1234m,
				Longitude = 1234f, Latitude = 1234f,
				TempId1 = 1234, TempId2 = 1234,
				BigPrecision = 1234d,
				ChildObject = (object)null,
				ChildModel1 = (OtherModel)null,
				ChildModel2 = (OtherModel)null
			});
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveModel_WhenCreatingObject()
		{
			When(Preparing, CreatingObject);

			Then.Model.Should().NotBeNull();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveListOfModels_WhenCreatingObjects()
		{
			Given.HowMany = 10;

			When(Preparing, CreatingObjects);

			Then.Models.Should().HaveCount(10);
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenCreatingModel_GivenDidNotCallPrepare()
		{
			WhenLastActionDeferred(CreatingModel);

			ThenLastAction.ShouldThrow<InvalidOperationException>();
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenCreatingModels_GivenDidNotCallPrepare()
		{
			Given.HowMany = 10;

			WhenLastActionDeferred(CreatingModels);

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

		private void CreatingObject()
		{
			Then.Model = (TestModel)Then.Target.CreateObject();
			
		}

		private void CreatingObjects()
		{
			Then.Models = (IEnumerable<TestModel>)Then.Target.CreateObject((int)Given.HowMany);
		}

		public sealed class Thens
		{
			public AutoModelFactory<TestModel> Target;
			public TestModel Model;
			public IEnumerable<TestModel> Models;
		}
	}
}