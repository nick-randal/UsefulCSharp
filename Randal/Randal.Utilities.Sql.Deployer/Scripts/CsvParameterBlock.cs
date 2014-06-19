using System;
using System.Collections.Generic;

namespace Randal.Utilities.Sql.Deployer.Scripts
{
	public abstract class CsvParameterBlock : BaseScriptBlock
	{
		protected CsvParameterBlock(string keyword, string text)
			: base(keyword, text)
		{

		}

		public override IReadOnlyList<string> Parse()
		{
			return Text.Split(CatalogSplit, StringSplitOptions.RemoveEmptyEntries);
		}

		private static readonly char[] CatalogSplit = { ',' };
	}
}
