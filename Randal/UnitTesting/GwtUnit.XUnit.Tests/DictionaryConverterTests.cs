// Useful C#
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
using Xunit;

namespace GwtUnit.XUnit.Tests;

public sealed class DictionaryConverterTests
{
	[Fact, PositiveTest]
	public void ShouldHaveConvertersForDictionaryTypesWhenCreating()
	{
		WhenCreating();

		Then.Converter.Should().BeAssignableTo<IDynamicEntityConverter>();
		Then.Converter.HasConverters.Should().BeTrue();
		Then.Converter.ConverterCount.Should().Be(5);
	}

	[Fact, PositiveTest]
	public void ShouldHaveValidInstanceWhenConvertingToDictionary()
	{
		Given.DataDictionary = new Dictionary<string, object> { { "Name", "Jane Doe" } };
		Given.ConversionTo = typeof(Dictionary<string, object>);

		WhenConverting();

		Then.Result.Should().NotBeNull();
		Then.Success.Should().BeTrue();
		Then.Result.As<Dictionary<string, object>>().Should().ContainKey("Name");
	}

	[Fact, PositiveTest]
	public void ShouldHaveValidInstanceWhenConvertingToIDictionary()
	{
		Given.DataDictionary = new Dictionary<string, object> { { "Name", "Jane Doe" } };
		Given.ConversionTo = typeof(IDictionary<string, object>);

		WhenConverting();

		Then.Result.Should().NotBeNull();
		Then.Success.Should().BeTrue();
		Then.Result.As<IDictionary<string, object>>().Should().ContainKey("Name");
	}

	[Fact, PositiveTest]
	public void ShouldHaveValidInstanceWhenConvertingToIReadOnlyDictionary()
	{
		Given.DataDictionary = new Dictionary<string, object> { { "Name", "Jane Doe" } };
		Given.ConversionTo = typeof(IReadOnlyDictionary<string, object>);

		WhenConverting();

		Then.Result.Should().NotBeNull();
		Then.Success.Should().BeTrue();
		Then.Result.As<IReadOnlyDictionary<string, object>>().Keys.Should().Contain("Name");
	}

	[Fact, PositiveTest]
	public void ShouldHaveValidInstanceWhenConvertingToICollection()
	{
		Given.DataDictionary = new Dictionary<string, object> { { "Name", "Jane Doe" } };
		Given.ConversionTo = typeof(ICollection<KeyValuePair<string, object>>);

		WhenConverting();

		Then.Result.Should().NotBeNull();
		Then.Success.Should().BeTrue();
		Then.Result.As<ICollection<KeyValuePair<string, object>>>().First().Key.Should().Be("Name");
	}

	[Fact, PositiveTest]
	public void ShouldHaveValidInstanceWhenConvertingToIEnumerable()
	{
		Given.DataDictionary = new Dictionary<string, object> { { "Name", "Jane Doe" } };
		Given.ConversionTo = typeof(IEnumerable<KeyValuePair<string, object>>);

		WhenConverting();

		Then.Result.Should().NotBeNull();
		Then.Success.Should().BeTrue();
		Then.Result.As<IEnumerable<KeyValuePair<string, object>>>().First().Key.Should().Be("Name");
	}

	private void WhenCreating()
	{
		Then.Converter = new DictionaryConverter();
	}

	private void WhenConverting()
	{
		WhenCreating();
		Then.Success = Then.Converter.TryConversion(Given.ConversionTo, Given.DataDictionary, out Then.Result);
	}

	private Givens Given { get; } = new();
	
	private Thens Then { get; } = new();

	private class Thens
	{
		public DictionaryConverter Converter = null!;
		public object Result = null!;
		public bool Success;
	}

	private class Givens
	{
		public Type ConversionTo = null!;
		public Dictionary<string, object> DataDictionary = null!;
	}
}