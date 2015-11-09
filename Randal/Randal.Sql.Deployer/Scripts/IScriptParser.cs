using System;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Sql.Deployer.Scripts
{
	public interface IScriptParser : IScriptParserConsumer
	{
		void AddRule(string keyword, Func<string, IScriptBlock> blockRule);
		void SetFallbackRule(Func<string, string, IScriptBlock> fallbackRule);
	}
}