// Useful C#
// Copyright (C) 2014-2016 Nicholas Randal
// 
// Useful C# is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version "b" of the License, or
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
	using ItemWithDependcies = Tuple<string, List<string>>;

	[TestClass]
	public sealed class DependencyListBuilderOfStringTests : UnitTestBase<DependencyListBuilderOfStringTests.Thens>
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
				.WithItem("A").IsDepedentOn("d")
				.WithItem("B").IsDepedentOn("d")
				.WithItem("C").IsDepedentOn("d")
				.WithItems("D", "E")
				.Build();

			When(BuildingDependencies);

			Then.OrderedList.Should().HaveCount(5);
			Then.OrderedList.Select(x => x.Item1).Should().Equal("D", "A", "B", "C", "E");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveListOfValues_WhenBuildingDependencies_GivenValues()
		{
			Given.Values = new Builder()
				.WithItem(".")
				.WithItem("A").IsDepedentOn("b", "C", "d")
				.WithItem("B").IsDepedentOn("i", "e")
				.WithItem("C").IsDepedentOn("h")
				.WithItem("D").IsDepedentOn("g")
				.WithItem("E").IsDepedentOn("f")
				.WithItems("F", "G", "H", "I")
				.Build();

			When(BuildingDependencies);

			Then.DependencyListBuilder.Should().NotBeNull();
			Then.DependencyListBuilder.OriginalValues.Should().HaveCount(10);
			Then.OrderedList.Should().HaveCount(10);

			Then.DependencyListBuilder.OriginalValues.Select(x => x.Item1).Should().Equal(".", "A", "B", "C", "D", "E", "F", "G", "H", "I");
			Then.OrderedList.Select(x => x.Item1).Should().Equal(".", "I", "F", "E", "B", "H", "C", "G", "D", "A");
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenBuildingDependencies_GivenCircularReference()
		{
			Given.Values = new Builder()
				.WithItem("A").IsDepedentOn("b")
				.WithItem("B").IsDepedentOn("e")
				.WithItem("E").IsDepedentOn("D", "A")
				.WithItems("C", "D")
				.Build();

			WhenLastActionDeferred(BuildingDependencies);

			ThenLastAction.ShouldThrow<InvalidOperationException>("a circular reference was defined.")
				.WithMessage("a circular reference was detected.  Circular path: \r\nA -> B -> E -> A");
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenBuildingDependencies_GivenNonExistentDependency()
		{
			Given.Values = new Builder().WithItem("A").IsDepedentOn("b").Build();

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

		[TestMethod, NegativeTest]
		public void ShouldThrownException_WhenBuildingDependencies_GivenMixedCaseAndCaseSensitivity()
		{
			Given.Values = new Builder().WithItem("A").IsDepedentOn("b").WithItem("B").Build();
			Given.Comparer = StringComparer.CurrentCulture;

			WhenLastActionDeferred(BuildingDependencies);

			ThenLastAction.ShouldThrow<KeyNotFoundException>().WithMessage("Item with key 'A' has dependency 'b', which was not found.");
		}

		protected override void Creating()
		{
			Then.DependencyListBuilder = new DependencyListBuilder<string, ItemWithDependcies>(Given.Values);
		}

		private void BuildingDependencies()
		{
			Func<ItemWithDependcies, string> getKeyFunc = x => x.Item1;
			Func<ItemWithDependcies, IEnumerable<string>> getDependenciesFunc = x => x.Item2;

			var comparer = Given.Comparer ?? StringComparer.OrdinalIgnoreCase;
			Then.OrderedList = Then.DependencyListBuilder.BuildDependencyList(getKeyFunc, getDependenciesFunc, comparer);
		}

		private sealed class Builder
		{
			public Builder WithItem(string item)
			{
				_list.Add(new ItemWithDependcies(item, new List<string>()));	
				return this;
			}

			public Builder WithItems(params string[] items)
			{
				_list.AddRange(items.Select(x => new ItemWithDependcies(x, new List<string>())));
				return this;
			}

			public Builder IsDepedentOn(params string[] dependencies)
			{
				_list.Last().Item2.AddRange(dependencies);
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
			public DependencyListBuilder<string, ItemWithDependcies> DependencyListBuilder;
			public List<ItemWithDependcies> OrderedList;
		}
	}
}