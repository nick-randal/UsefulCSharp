using System.Collections.Generic;

namespace Randal.Utilities.Sql.Deployer.Scripts
{
	public interface IScriptBlock
	{
		string Keyword { get; }
		string Text { get; }
		bool IsValid { get; }
		IReadOnlyList<string> Parse();
	}

	public abstract class BaseScriptBlock : IScriptBlock
	{
		protected BaseScriptBlock(string keyword, string text)
		{
			Keyword = keyword;
			Text = text;
		}

		public string Text
		{
			get { return _text; }
			protected set { _text = (value ?? string.Empty).Trim(); }
		}

		public string Keyword
		{
			get { return _keyword; }
			protected set { _keyword = (value ?? string.Empty).Trim(); }
		}

		public bool IsValid { get; protected set; }

		public virtual IReadOnlyList<string> Parse()
		{
			return EmptyStringList;
		}

		protected static readonly IReadOnlyList<string> EmptyStringList = new List<string>(); 
		private string _keyword, _text;
	}
}