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
using Randal.Core.Dynamic;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.Dynamic
{
	[TestClass]
	public class DynamicEntityConverterTests
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveNoConvertesWhenCreating()
		{
			ThenConverter.Should().BeAssignableTo<IDynamicEntityConverter>();
			ThenConverter.HasConverters.Should().BeFalse();
			ThenConverter.ConverterCount.Should().Be(0);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveNullResultWhenConvertingGivenNoValidConverters()
		{
			GivenDataDictionary = new Dictionary<string, object> {{"Name", "Jane Doe"}};
			GivenConversionTo = typeof (string);

			WhenConverting();

			ThenResult.Should().BeNull();
			ThenSuccess.Should().BeFalse();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveRegisteredConverterWhenConverterIsAdded()
		{
			GivenConversionTo = typeof (string);
			GivenConverter = dct => string.Join(", ", dct.Keys.Select(key => key + "=" + dct[key]));

			WhenAddingConverter();

			ThenConverter.HasConverters.Should().BeTrue();
			ThenConverter.ConverterCount.Should().Be(1);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveRegisteredConverterWhenConverterIsAddedUsingGenericMethod()
		{
			GivenConverter = dct => string.Join(", ", dct.Keys.Select(key => key + "=" + dct[key]));

			WhenAddingConverter<string>();

			ThenConverter.HasConverters.Should().BeTrue();
			ThenConverter.ConverterCount.Should().Be(1);
		}

		[TestMethod, PositiveTest]
		public void ShouldNotHaveConverterWhenConverterIsRemoved()
		{
			GivenConversionTo = typeof (string);
			GivenConverter = dct => string.Join(", ", dct.Keys.Select(key => key + "=" + dct[key]));

			WhenRemovingConverter();

			ThenConverter.HasConverters.Should().BeFalse();
			ThenConverter.ConverterCount.Should().Be(0);
			ThenRemovedConverter.Should().NotBeNull();
		}

		[TestMethod, PositiveTest]
		public void ShouldNotHaveConverterWhenConverterIsRemovedUsingGenericMethod()
		{
			GivenConverter = dct => string.Join(", ", dct.Keys.Select(key => key + "=" + dct[key]));

			WhenRemovingConverter<string>();

			ThenConverter.HasConverters.Should().BeFalse();
			ThenConverter.ConverterCount.Should().Be(0);
			ThenRemovedConverter.Should().NotBeNull();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveNullWhenRemovingNonExistentConverter()
		{
			WhenRemovingConverter<string>();

			ThenConverter.HasConverters.Should().BeFalse();
			ThenConverter.ConverterCount.Should().Be(0);
			ThenRemovedConverter.Should().BeNull();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValidResultWhenConvertingToAKnownConverterType()
		{
			GivenDataDictionary = new Dictionary<string, object> {{"Name", "Jane Doe"}};
			GivenConversionTo = typeof (string);
			GivenConverter = dct => string.Join(", ", dct.Keys.Select(key => key + "=" + dct[key]));

			WhenConverting();

			ThenSuccess.Should().BeTrue();
			ThenResult.Should().BeAssignableTo<string>();
			ThenResult.Should().NotBeNull().And.Be("Name=Jane Doe");
		}

		[TestInitialize]
		public void TestSetup()
		{
			GivenConverter = null;
			ThenConverter = new DynamicEntityConverter();
		}

		private void WhenAddingConverter<TConversionType>()
		{
			if (GivenConverter != null)
				ThenConverter.AddTypeConverter<TConversionType>(GivenConverter);
		}

		private void WhenAddingConverter()
		{
			if (GivenConverter != null)
				ThenConverter.AddTypeConverter(GivenConversionTo, GivenConverter);
		}

		private void WhenRemovingConverter<TConversionType>()
		{
			WhenAddingConverter<TConversionType>();
			ThenRemovedConverter = ThenConverter.RemoveTypeConverter<TConversionType>();
		}

		private void WhenRemovingConverter()
		{
			WhenAddingConverter();
			ThenRemovedConverter = ThenConverter.RemoveTypeConverter(GivenConversionTo);
		}

		private void WhenConverting()
		{
			if (GivenConverter != null)
				ThenConverter.AddTypeConverter(GivenConversionTo, GivenConverter);
			ThenSuccess = ThenConverter.TryConversion(GivenConversionTo, GivenDataDictionary, out ThenResult);
		}

		private Type GivenConversionTo;
		private Dictionary<string, object> GivenDataDictionary;
		private Func<Dictionary<string, object>, object> GivenConverter;
		private DynamicEntityConverter ThenConverter;
		private Func<Dictionary<string, object>, object> ThenRemovedConverter;
		private object ThenResult;
		private bool ThenSuccess;
	}
}