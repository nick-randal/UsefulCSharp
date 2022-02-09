﻿// Useful C#
// Copyright (C) 2014-2022 Nicholas Randal
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
using System.Linq;
using FluentAssertions;
using GwtUnit.XUnit;
using Xunit;

namespace Randal.Core.Testing.Factory.Tests
{
	/// <summary>
	/// Created by nrandal on 9/10/2015 10:24:57 AM
	/// </summary>
	public sealed class AutoModelFactoryTests : XUnitTestBase<AutoModelFactoryTests.Thens>
	{
		[Fact, PositiveTest]
		public void ShouldHaveValidInstance_WhenCreating()
		{
			When(Creating);

			Then.Target.Should().NotBeNull();
		}

		[Fact, PositiveTest]
		public void ShouldHaveValidInstance_WhenCreatingAndPreparing()
		{
			When(NotCreating, CreatingAndPreparing);

			Then.Target.Should().NotBeNull();
		}

		[Fact, PositiveTest]
		public void ShouldNotThrowException_WhenPreparing()
		{
			WhenLastActionDeferred(Preparing);

			ThenLastAction.Should().NotThrow();
		}

		[Fact, PositiveTest]
		public void ShouldHaveModelWithValues_WhenCreatingModel_GivenGenericIncrementingValues()
		{
			When(Preparing, CreatingModel);

			Then.Model.Should().BeEquivalentTo(new
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

		[Fact, PositiveTest]
		public void ShouldHaveListOfModels_WhenCreatingModels_GivenGenericIncrementingValues()
		{
			Given.HowMany = 1234;

			When(Preparing, CreatingIEnumerable);

			Then.Models.Should().HaveCount(1234);
			Then.Models.Last().Should().BeEquivalentTo(new
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

		[Fact, PositiveTest]
		public void ShouldHaveList_WhenCreatingList()
		{
			Given.HowMany = 100;

			When(Preparing, CreatingList);

			Then.ListOfModels.Should().NotBeNull().And.HaveCount(100);
		}

		[Fact, PositiveTest]
		public void ShouldHaveModel_WhenCreatingObject()
		{
			When(Preparing, CreatingObject);

			Then.Model.Should().NotBeNull();
		}

		[Fact, PositiveTest]
		public void ShouldHaveListOfModels_WhenCreatingObjects()
		{
			Given.HowMany = 10;

			When(Preparing, CreatingObjects);

			Then.Models.Should().HaveCount(10);
		}

		[Fact, NegativeTest]
		public void ShouldThrowException_WhenCreatingModel_GivenDidNotCallPrepare()
		{
			WhenLastActionDeferred(CreatingModel);

			ThenLastAction.Should().Throw<InvalidOperationException>();
		}

		[Fact, NegativeTest]
		public void ShouldThrowException_WhenCreatingIEnumerable_GivenDidNotCallPrepare()
		{
			Given.HowMany = 10;

			WhenLastActionDeferred(CreatingIEnumerable);

			ThenLastAction.Should().Throw<InvalidOperationException>();
		}

		protected override void Creating()
		{
			Then.Target = new AutoModelFactory<TestModel>(Given.Values as IValueFactory);
		}

		private void CreatingAndPreparing()
		{
			Then.Target = AutoModelFactory<TestModel>.CreateAndPrepare();
		}

		private void Preparing()
		{
			Then.Target.Prepare();
		}

		private void CreatingModel()
		{
			Then.Model = Then.Target.Create();
		}

		private void CreatingIEnumerable()
		{
			Then.Models = Then.Target.Create((int)Given.HowMany);
		}

		private void CreatingList()
		{
			Then.ListOfModels = Then.Target.CreateList((int)Given.HowMany);
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
			public List<TestModel> ListOfModels;
		}
	}
}