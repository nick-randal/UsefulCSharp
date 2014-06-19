/*
Useful C#
Copyright (C) 2014  Nicholas Randal

Useful C# is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/

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