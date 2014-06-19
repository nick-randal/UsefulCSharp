using System.Collections.Generic;
using Newtonsoft.Json;

namespace Randal.Utilities.Sql.Deployer.Scripts
{
	public sealed class ConfigurationBlock : BaseScriptBlock
	{
		public ConfigurationBlock(string json = null) : base("configuration", json)
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