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

using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace GwtUnit.XUnit.Tests;

public sealed class NullConverterTests
{
	[Fact, PositiveTest]
	public void ShouldHaveNoConvertersWhenCreatingOfNullConverter()
	{
		WhenCreating();

		ThenConverter.Should().BeAssignableTo<IDynamicEntityConverter>();
		ThenConverter.HasConverters.Should().BeFalse();
		ThenConverter.ConverterCount.Should().Be(0);
	}

	[Fact, PositiveTest]
	public void ShouldHaveNullResultWhenConverting()
	{
		GivenDataDictionary = new Dictionary<string, object> {{"Name", "Jane Doe"}};
		WhenConverting();
		ThenResult.Should().BeNull();
		ThenSuccess.Should().BeFalse();
	}

	private void WhenCreating()
	{
		ThenConverter = new NullConverter();
	}

	private void WhenConverting()
	{
		WhenCreating();
		ThenSuccess = ThenConverter.TryConversion(typeof (IDictionary<string, object>), GivenDataDictionary, out ThenResult);
	}

	private Dictionary<string, object> GivenDataDictionary;
	private NullConverter ThenConverter;
	private object ThenResult;
	private bool ThenSuccess;
}