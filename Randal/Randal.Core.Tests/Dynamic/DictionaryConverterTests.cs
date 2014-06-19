using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;

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
			GivenDataDictionary = new Dictionary<string, object> { { "Name", "Jane Doe" } };
			GivenConversionTo = typeof(Dictionary<string, object>);

			WhenConverting();
			
			ThenResult.Should().NotBeNull();
			ThenSuccess.Should().BeTrue();
			ThenResult.As<Dictionary<string, object>>().Should().ContainKey("Name");
		}

		[TestMethod]
		public void ShouldHaveValidInstanceWhenConvertingToIDictionary()
		{
			GivenDataDictionary = new Dictionary<string, object> { { "Name", "Jane Doe" } };
			GivenConversionTo = typeof(IDictionary<string, object>);

			WhenConverting();

			ThenResult.Should().NotBeNull();
			ThenSuccess.Should().BeTrue();
			ThenResult.As<IDictionary<string, object>>().Should().ContainKey("Name");
		}

		[TestMethod]
		public void ShouldHaveValidInstanceWhenConvertingToIReadOnlyDictionary()
		{
			GivenDataDictionary = new Dictionary<string, object> { { "Name", "Jane Doe" } };
			GivenConversionTo = typeof(IReadOnlyDictionary<string, object>);

			WhenConverting();

			ThenResult.Should().NotBeNull();
			ThenSuccess.Should().BeTrue();
			ThenResult.As<IReadOnlyDictionary<string, object>>().Keys.Should().Contain("Name");
		}
		
		[TestMethod]
		public void ShouldHaveValidInstanceWhenConvertingToICollection()
		{
			GivenDataDictionary = new Dictionary<string, object> { { "Name", "Jane Doe" } };
			GivenConversionTo = typeof(ICollection<KeyValuePair<string, object>>);

			WhenConverting();

			ThenResult.Should().NotBeNull();
			ThenSuccess.Should().BeTrue();
			ThenResult.As<ICollection<KeyValuePair<string, object>>>().First().Key.Should().Be("Name");
		}
		
		[TestMethod]
		public void ShouldHaveValidInstanceWhenConvertingToIEnumerable()
		{
			GivenDataDictionary = new Dictionary<string, object> { { "Name", "Jane Doe" } };
			GivenConversionTo = typeof(IEnumerable<KeyValuePair<string, object>>);

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
