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
using Randal.Core.Dynamic;

namespace Randal.Tests.Core.Dynamic
{
	[TestClass]
	public sealed class DictionaryConverterTests
	{
		[TestMethod]
		public void ShouldHaveConvertersForDictionaryTypesWhenCreating()
		{
			WhenCreating();

			ThenConverter.Should().BeAssignableTo<IDynamicEntityConverter>();
			ThenConverter.HasConverters.Should().BeTrue();
			ThenConverter.ConverterCount.Should().Be(5);
		}

		[TestMethod]
		public void ShouldHaveValidInstanceWhenConvertingToDictionary()
		{
			GivenDataDictionary = new Dictionary<string, object> {{"Name", "Jane Doe"}};
			GivenConversionTo = typeof (Dictionary<string, object>);

			WhenConverting();

			ThenResult.Should().NotBeNull();
			ThenSuccess.Should().BeTrue();
			ThenResult.As<Dictionary<string, object>>().Should().ContainKey("Name");
		}

		[TestMethod]
		public void ShouldHaveValidInstanceWhenConvertingToIDictionary()
		{
			GivenDataDictionary = new Dictionary<string, object> {{"Name", "Jane Doe"}};
			GivenConversionTo = typeof (IDictionary<string, object>);

			WhenConverting();

			ThenResult.Should().NotBeNull();
			ThenSuccess.Should().BeTrue();
			ThenResult.As<IDictionary<string, object>>().Should().ContainKey("Name");
		}

		[TestMethod]
		public void ShouldHaveValidInstanceWhenConvertingToIReadOnlyDictionary()
		{
			GivenDataDictionary = new Dictionary<string, object> {{"Name", "Jane Doe"}};
			GivenConversionTo = typeof (IReadOnlyDictionary<string, object>);

			WhenConverting();

			ThenResult.Should().NotBeNull();
			ThenSuccess.Should().BeTrue();
			ThenResult.As<IReadOnlyDictionary<string, object>>().Keys.Should().Contain("Name");
		}

		[TestMethod]
		public void ShouldHaveValidInstanceWhenConvertingToICollection()
		{
			GivenDataDictionary = new Dictionary<string, object> {{"Name", "Jane Doe"}};
			GivenConversionTo = typeof (ICollection<KeyValuePair<string, object>>);

			WhenConverting();

			ThenResult.Should().NotBeNull();
			ThenSuccess.Should().BeTrue();
			ThenResult.As<ICollection<KeyValuePair<string, object>>>().First().Key.Should().Be("Name");
		}

		[TestMethod]
		public void ShouldHaveValidInstanceWhenConvertingToIEnumerable()
		{
			GivenDataDictionary = new Dictionary<string, object> {{"Name", "Jane Doe"}};
			GivenConversionTo = typeof (IEnumerable<KeyValuePair<string, object>>);

			WhenConverting();

			ThenResult.Should().NotBeNull();
			ThenSuccess.Should().BeTrue();
			ThenResult.As<IEnumerable<KeyValuePair<string, object>>>().First().Key.Should().Be("Name");
		}


		private void WhenCreating()
		{
			ThenConverter = new DictionaryConverter();
		}

		private void WhenConverting()
		{
			WhenCreating();
			ThenSuccess = ThenConverter.TryConversion(GivenConversionTo, GivenDataDictionary, out ThenResult);
		}

		private Type GivenConversionTo;
		private Dictionary<string, object> GivenDataDictionary;
		private DictionaryConverter ThenConverter;
		private object ThenResult;
		private bool ThenSuccess;
	}
}