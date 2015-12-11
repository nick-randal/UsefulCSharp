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
using Newtonsoft.Json;

namespace Randal.Sql.Deployer.Scripts.Blocks
{
	public sealed class OptionsBlock : BaseScriptBlock
	{
		public OptionsBlock(string json = null) : base(ScriptConstants.Blocks.Options, json)
		{
			if (json != null)
				return;

			Text = "Script settings initialized through direct assignment.";
			Settings = new ScriptSettings();
			IsValid = true;
		}

		public ScriptSettings Settings { get; private set; }

		public override IReadOnlyList<string> Parse()
		{
			if (Settings != null)
				return EmptyStringList;

			try
			{
				if (Text.StartsWith("{") == false)
					Text = '{' + Text + '}';

				Settings = JsonConvert.DeserializeObject<ScriptSettings>(Text);
				IsValid = true;
			}
			catch (JsonReaderException)
			{
				IsValid = false;
			}

			return EmptyStringList;
		}
	}
}