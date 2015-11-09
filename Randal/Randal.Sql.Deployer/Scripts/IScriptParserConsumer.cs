using System.Collections.Generic;

namespace Randal.Sql.Deployer.Scripts
{
	public interface IScriptParserConsumer
	{
		bool HasFallbackRule { get; }
		IReadOnlyList<string> RegisteredKeywords();
		SourceScript Parse(string name, string text);
	}
}