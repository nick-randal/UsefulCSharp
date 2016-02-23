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
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Structures;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.Structures
{
	using ItemWithDependcies = Tuple<int, List<int>>;

	[TestClass]
	public sealed class DependencyListBuilderOfIntsTests : UnitTestBase<DependencyListBuilderOfIntsTests.Thens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveEmptyList_WhenCreatingNewObject()
		{
			Given.Values = new Builder().Build();

			When(Creating);

			Then.DependencyListBuilder.Should().NotBeNull();
			Then.DependencyListBuilder.OriginalValues.Should().BeEmpty();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveListOfValues_WhenBuildingDependencies_GivenValuesReferencingSameDependency()
		{
			Given.Values = new Builder()
				.WithItem(1).IsDepedentOn(4)
				.WithItem(2).IsDepedentOn(4)
				.WithItem(3).IsDepedentOn(4)
				.WithItems(4, 5)
				.Build();

			When(BuildingDependencies);

			Then.OrderedList.Should().HaveCount(5);
			Then.OrderedList.Select(x => x.Item1).Should().Equal(new[] { 4, 1, 2, 3, 5 });
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveListOfValues_WhenBuildingDependencies_GivenValues()
		{
			Given.Values = new Builder()
				.WithItem(0)
				.WithItem(1).IsDepedentOn(2, 3, 4)
				.WithItem(2).IsDepedentOn(9, 5)
				.WithItem(3).IsDepedentOn(8)
				.WithItem(4).IsDepedentOn(7)
				.WithItem(5).IsDepedentOn(6)
				.WithItems(6, 7, 8, 9)
				.Build();

			When(BuildingDependencies);

			Then.DependencyListBuilder.Should().NotBeNull();
			Then.DependencyListBuilder.OriginalValues.Should().HaveCount(10);
			Then.OrderedList.Should().HaveCount(10);

			Then.DependencyListBuilder.OriginalValues.Select(x => x.Item1).Should().Equal(new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9});
			Then.OrderedList.Select(x => x.Item1).Should().Equal(new[] {0, 9, 6, 5, 2, 8, 3, 7, 4, 1});
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenBuildingDependencies_GivenCircularReference()
		{
			Given.Values = new Builder()
				.WithItem(1).IsDepedentOn(2)
				.WithItem(2).IsDepedentOn(5)
				.WithItem(5).IsDepedentOn(4, 1)
				.WithItems(3, 4).Build();

			WhenLastActionDeferred(BuildingDependencies);

			ThenLastAction.ShouldThrow<InvalidOperationException>("a circular reference was defined.")
				.WithMessage("a circular reference was detected.  Circular path: \r\n1 -> 2 -> 5 -> 1");
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenBuildingDependencies_GivenNonExistentDependency()
		{
			Given.Values = new Builder().WithItem(1).IsDepedentOn(2).Build();

			WhenLastActionDeferred(BuildingDependencies);

			ThenLastAction.ShouldThrow<KeyNotFoundException>();
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenCreating_GivenNullValues()
		{
			Given.Values = null;

			WhenLastActionDeferred(Creating);

			ThenLastAction.ShouldThrow<ArgumentNullException>();
		}

		protected override void Creating()
		{
			Then.DependencyListBuilder = new DependencyListBuilder<int, ItemWithDependcies>(Given.Values);
		}

		private void BuildingDependencies()
		{
			Func<ItemWithDependcies, int> getKeyFunc = x => x.Item1;
			Func<ItemWithDependcies, IEnumerable<int>> getDependenciesFunc = x => x.Item2;

			Then.OrderedList = Then.DependencyListBuilder.BuildDependencyList(getKeyFunc, getDependenciesFunc);
		}

		private sealed class Builder
		{
			public Builder WithItem(int item)
			{
				_list.Add(new ItemWithDependcies(item, new List<int>()));	
				return this;
			}

			public Builder WithItems(params int[] items)
			{
				_list.AddRange(items.Select(x => new ItemWithDependcies(x, new List<int>())));
				return this;
			}

			public Builder IsDepedentOn(params int[] dependencies)
			{
				_list[_list.Count - 1].Item2.AddRange(dependencies);
				return this;
			}

			public IReadOnlyList<ItemWithDependcies> Build()
			{
				return _list.AsReadOnly();
			}

			private readonly List<ItemWithDependcies> _list = new List<ItemWithDependcies>();
		}

		public sealed class Thens
		{
			public DependencyListBuilder<int, ItemWithDependcies> DependencyListBuilder;
			public List<ItemWithDependcies> OrderedList;
		}
	}
}