using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Dynamic;
using System.Collections.Generic;

namespace Randal.Tests.Core.Dynamic
{
	[TestClass]
	public class NullConverterTests
	{
		[TestMethod]
		public void ShouldHaveNoConvertersWhenCreatingOfNullConverter()
		{
			WhenCreating();

			ThenConverter.Should().BeAssignableTo<IDynamicEntityConverter>();
			ThenConverter.HasConverters.Should().BeFalse();
			ThenConverter.ConverterCount.Should().Be(0);
		}

		[TestMethod]
		public void ShouldHaveNullResultWhenConverting()
		{
			GivenDataDictionary = new Dictionary<string, object> { { "Name", "Jane Doe" } };
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
			ThenSuccess = ThenConverter.TryConversion(typeof(IDictionary<string, object>), GivenDataDictionary, out ThenResult);
		}

		private Dictionary<string, object> GivenDataDictionary;
		private NullConverter ThenConverter;
		private object ThenResult;
		private bool ThenSuccess;
	}
}
