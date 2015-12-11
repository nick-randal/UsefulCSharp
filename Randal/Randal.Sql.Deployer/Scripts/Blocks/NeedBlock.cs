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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Randal.Sql.Deployer.Scripts.Blocks
{
	public sealed class NeedBlock : CsvParameterBlock
	{
		public NeedBlock(string text) : base(ScriptConstants.Blocks.Need, text)
		{
			_files = new List<string>();
		}

		public IReadOnlyList<string> Files
		{
			get { return _files; }
		}

		public override IReadOnlyList<string> Parse()
		{
			var messages = new List<string>();

			foreach (var item in base.Parse())
			{
				var temp = item.Trim();

				if (InvalidFileName.IsMatch(temp))
				{
					messages.Add("Invalid file name '" + temp + "' provided as a 'need'.");
					continue;
				}

				if (temp.EndsWith(ScriptConstants.SqlExtension, StringComparison.InvariantCultureIgnoreCase))
					_files.Add(temp.Replace(ScriptConstants.SqlExtension, string.Empty));
				else
					_files.Add(temp);
			}

			IsValid = messages.Count == 0;

			return messages;
		}

		private readonly List<string> _files;

		private static readonly Regex InvalidFileName =
			new Regex("[" + Regex.Escape(string.Join(string.Empty, Path.GetInvalidFileNameChars())) + "]",
				RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
	}
}