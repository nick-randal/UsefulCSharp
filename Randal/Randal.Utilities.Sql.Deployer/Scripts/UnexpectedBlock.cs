using System.Collections.Generic;

namespace Randal.Utilities.Sql.Deployer.Scripts
{
	public sealed class UnexpectedBlock : BaseScriptBlock
	{
		public UnexpectedBlock(string keyword, string text) : base(keyword, text)
		{
		}

		public override IReadOnlyList<string> Parse()
		{
			return new List<string> { "Unexpected keyword '" + Keyword + "' found for this block." };
		}
	}
}