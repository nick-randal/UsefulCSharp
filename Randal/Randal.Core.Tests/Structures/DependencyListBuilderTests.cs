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
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Structures;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.Structures
{
	using ItemWithDependcies = Tuple<int, List<int>>;

	public sealed class PositiveTest : TestCategoryBaseAttribute
	{
		public override IList<string> TestCategories
		{
			get { return List; }
		}

		private static readonly List<string> List = new List<string> { "Postive" };
	}

	public sealed class NegativeTest : TestCategoryBaseAttribute
	{
		public override IList<string> TestCategories
		{
			get { return List; }
		}

		private static readonly List<string> List = new List<string> { "Postive" };
	}

	[TestClass]
	public sealed class DependencyListBuilderTests : BaseUnitTest<DependencyListBuilderThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveEmptyListWhenCreatingNewGraph()
		{
			Given.Values = new Builder().Build();

			When(Creating);

			Then.DependencyListBuilder.Should().NotBeNull();
			Then.DependencyListBuilder.OriginalValues.Should().BeEmpty();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveListOfValuesWhenBuildingDependenciesGivenValues()
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

			Then.DependencyListBuilder.OriginalValues.Select(x => x.Item1).Should().BeEquivalentTo(new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9});
			Then.OrderedList.Select(x => x.Item1).Should().Equal(new[] {0, 9, 6, 5, 2, 8, 3, 7, 4, 1});
		}

		[TestMethod, TestCategory("Negative")]
		public void ShouldThrowExceptionWhenBuildingDependenciesGivenCircularReference()
		{
			Given.Values = new Builder()
				.WithItem(1).IsDepedentOn(2)
				.WithItem(2).IsDepedentOn(3, 4)
				.WithItem(3).IsDepedentOn(1, 2)
				.WithItem(4).Build();

			ThrowsExceptionWhen(BuildingDependencies);

			ThenLastAction.ShouldThrow<InvalidOperationException>("a circular reference was defined.");
		}

		[TestMethod, TestCategory("Negative")]
		public void ShouldThrowExceptionWhenBuildingDependenciesGivenNonExistentDependency()
		{
			Given.Values = new Builder().WithItem(1).IsDepedentOn(2).Build();

			ThrowsExceptionWhen(BuildingDependencies);

			ThenLastAction.ShouldThrow<KeyNotFoundException>();
		}

		[TestMethod, TestCategory("Negative")]
		public void ShouldThrowExceptionWhenCreatingGivenNullValues()
		{
			Given.Values = null;

			ThrowsExceptionWhen(Creating);

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
	}

	public sealed class DependencyListBuilderThens
	{
		public DependencyListBuilder<int, ItemWithDependcies> DependencyListBuilder;
		public List<ItemWithDependcies> OrderedList;
	}
}