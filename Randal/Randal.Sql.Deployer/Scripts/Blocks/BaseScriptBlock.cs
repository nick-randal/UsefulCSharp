// Useful C#
// Copyright (C) 2014-2016 Nicholas Randal
// 
// Useful C# is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

using System.Collections.Generic;

namespace Randal.Sql.Deployer.Scripts.Blocks
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