using FluentAssertions;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Scripting;
using Rhino.Mocks;

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

		[TestMethod]
		public void ShouldHaveTextWhenFormattingStoredProcedure()
		{
			Given.Procedure = "spTest";
			When(Creating, Formatting);
			Then.Text.Should().Be("--:: catalog Test\r\n\r\n--:: ignore\r\nuse Test\r\n\r\n--:: pre\r\nexec coreCreateProcedure 'spTest'\r\nGO\r\n\r\n--:: main\r\n\r\n\r\n\r\n/*\r\n	exec spTest \r\n*/");
		}

		private void Creating()
		{
			var server = MockRepository.GenerateMock<IServer>();
			server.Stub(x => x.GetDependencies(Arg<SqlSmoObject>.Is.NotNull)).Return(new Urn[0]);
			Then.Formatter = new ScriptFormatter(server);
		}

		private void Formatting()
		{
			Then.Text = Then.Formatter.Format(new StoredProcedure(new Database(new Server(),  "Test"), Given.Procedure));
		}
	}

	public sealed class ScriptFormatterThens
	{
		public ScriptFormatter Formatter;
		public string Text;
	}
}