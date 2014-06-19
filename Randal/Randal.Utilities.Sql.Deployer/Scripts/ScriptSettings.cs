using Newtonsoft.Json;

namespace Randal.Utilities.Sql.Deployer.Scripts
{
	public sealed class ScriptSettings
	{
		public ScriptSettings() : this(30)
		{
			
		}

		public ScriptSettings(int timeout, bool useTransaction = true)
		{
			Timeout = timeout;
			UseTransaction = useTransaction;
		}

		[JsonProperty(Required = Required.Default)]
		public int Timeout { get; private set; }

		[JsonProperty(Required = Required.Default)]
		public bool UseTransaction { get; private set; }
	}
}