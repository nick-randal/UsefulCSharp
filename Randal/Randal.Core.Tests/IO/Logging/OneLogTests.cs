using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.IO.Logging;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.IO.Logging
{
	[TestClass]
	public sealed class OneLogTests : BaseUnitTest<object>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveStringLoggerWhenAccessingInstance()
		{
			OneLog.Inst.Should().NotBeNull().And.BeAssignableTo<StringLogger>();
		}
	}
}
