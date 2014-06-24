﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Structures;
using Randal.Core.Testing.UnitTest;
using System.Collections.Generic;

namespace Randal.Tests.Core.Structures
{
	[TestClass]
	public sealed class DependencyListBuilderTests : BaseUnitTest<DependencyListBuilderThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveEmptyListWhenCreatingNewGraph()
		{
			Given.Values = new Builder().Build();

			When(Creating);

			Then.Graph.Should().NotBeNull();
			Then.Graph.OriginalValues.Should().BeEmpty();
		}

		[TestMethod]
		public void ShouldHaveListOfValuesWhenBuildingDependenciesGivenValues()
		{
			Given.Values = new Builder()
				.WithItem(1, 2, 3, 4)
				.WithItem(2, 9, 5)
				.WithItem(3, 8)
				.WithItem(4, 7)
				.WithItem(5, 6)
				.WithItem(6).WithItem(7).WithItem(8).WithItem(9)
				.Build();

			When(Creating, BuildingDependencies);

			Then.Graph.Should().NotBeNull();
			Then.Graph.OriginalValues.Should().HaveCount(9);
			Then.List.Should().HaveCount(9);

			Then.Graph.OriginalValues.Select(x => x.Item1).Should().BeEquivalentTo(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9});
			Then.List.Select(x => x.Item1).Should().BeEquivalentTo(new[] { 1, 2, 9, 5, 6, 3, 8, 4, 7 });
		}

		[TestMethod]
		public void ShouldNotThrowExceptionWhenBuildingDependenciesGivenCircularReference()
		{
			Given.Values = new Builder()
				.WithItem(1, 2)
				.WithItem(2, 3, 4)
				.WithItem(3, 1, 2)
				.WithItem(4).Build();

			When(Creating, BuildingDependencies);

			Then.List.Should().HaveCount(4);
			Then.Graph.OriginalValues.Select(x => x.Item1).Should().BeEquivalentTo(new[] { 1, 2, 3, 4 });
			Then.List.Select(x => x.Item1).Should().BeEquivalentTo(new[] { 1, 2, 3, 4 });
		}

		[TestMethod, ExpectedException(typeof (KeyNotFoundException))]
		public void ShouldThrowExceptionWhenBuildingDependenciesGivenNonExistentDependency()
		{
			Given.Values = new Builder().WithItem(1, 2).Build();

			When(Creating, BuildingDependencies);
		}

		[TestMethod, ExpectedException(typeof (ArgumentNullException))]
		public void ShouldThrowExceptionWhenCreatingGivenNullValues()
		{
			Given.Values = null;

			When(Creating);
		}

		private void Creating()
		{
			Then.Graph = new DependencyListBuilder<int, Tuple<int, int[]>>(Given.Values);
		}

		private void BuildingDependencies()
		{
			Func<Tuple<int, int[]>, int> getKeyFunc = x => x.Item1;
			Func<Tuple<int, int[]>, IEnumerable<int>> getDependenciesFunc = x => x.Item2;

			Then.List = Then.Graph.BuildDependencyList(getKeyFunc, getDependenciesFunc);
		}


		sealed class Builder
		{
			public Builder()
			{
				_list = new List<Tuple<int, int[]>>();
			}

			public Builder WithItem(int key, params int[] dependencies)
			{
				_list.Add(new Tuple<int, int[]>(key, dependencies));

				return this;
			}

			public IReadOnlyList<Tuple<int, int[]>> Build()
			{
				return _list.AsReadOnly();
			}

			private readonly List<Tuple<int, int[]>> _list;
		}
	}

	public sealed class DependencyListBuilderThens
	{
		public DependencyListBuilder<int, Tuple<int, int[]>> Graph;
		public List<Tuple<int, int[]>> List;
	}
}
