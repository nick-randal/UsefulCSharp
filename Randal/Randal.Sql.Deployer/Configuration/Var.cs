using System.Xml.Serialization;

namespace Randal.Sql.Deployer.Configuration
{
	public sealed class Var
	{
		public Var() : this(string.Empty, string.Empty) { }
		public Var(string name, string value)
		{
			Name = name;
			Value = value;
		}

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string Value { get; set; }
	}
}