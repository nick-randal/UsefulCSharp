using System;

namespace Randal.Utilities.Sql.Deployer.Scripts
{
	public interface ICreateStandardParsers
	{
		IScriptParser CreateStandardParser();
	}

	public sealed class ScriptParserFactory : ICreateStandardParsers
	{
		public IScriptParser CreateStandardParser()
		{
			var parser = new ScriptParser();

			Func<string, IScriptBlock> createConfigurationBlock = txt => new ConfigurationBlock(txt);

			parser.AddRule("catalog", txt => new CatalogBlock(txt));
			parser.AddRule("config", createConfigurationBlock);
			parser.AddRule("configuration", createConfigurationBlock);
			parser.AddRule("options", createConfigurationBlock);
			parser.AddRule("need", txt => new NeedBlock(txt));
			parser.AddRule("ignore", txt => new IgnoreScriptBlock(txt));
			parser.AddRule("pre", txt => new SqlCommandBlock("pre", txt, SqlScriptPhase.Pre));
			parser.AddRule("main", txt => new SqlCommandBlock("main", txt, SqlScriptPhase.Main));
			parser.AddRule("post", txt => new SqlCommandBlock("post", txt, SqlScriptPhase.Post));

			parser.SetFallbackRule((kw, txt) => new UnexpectedBlock(kw, txt));

			return parser;
		}
	}
}