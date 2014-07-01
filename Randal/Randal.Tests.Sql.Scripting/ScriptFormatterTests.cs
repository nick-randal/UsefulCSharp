using FluentAssertions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Scripting;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class ScriptFormatterTests : BaseUnitTest<ScriptFormatterThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveValidScriptFormatterWhenCreatingInstace()
		{
			When(Creating);
			Then.Formatter.Should().NotBeNull().And.BeAssignableTo<IScriptFormatter>();
		}

		private void Creating()
		{
			Then.Formatter = new ScriptFormatter(new DependencyWalker());
		}
	}

	public sealed class ScriptFormatterThens
	{
		public ScriptFormatter Formatter;
	}
}