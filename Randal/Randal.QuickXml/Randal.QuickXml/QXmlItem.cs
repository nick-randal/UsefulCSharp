using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Randal.QuickXml
{
	public interface IQxmlItem
	{
		XmlNodeType Type { get; }
		int Depth { get; }
		string Name { get; }
		string Value { get; }
		XNode ToNode();
	}

	public abstract class QXmlItem : IQxmlItem
	{
		protected QXmlItem(XmlNodeType type, int depth, string name, string value)
		{
			Type = type;
			Depth = Depth;
			Name = name;
			Value = value;
		}

		public XmlNodeType Type { get; protected set; }
		public int Depth { get; protected set; }
		public virtual string Name { get; protected set; }
		public virtual string Value { get; protected set; }

		public abstract XNode ToNode();
	}

	public sealed class QAttribute : QXmlItem
	{
		public QAttribute(int depth, string name, string value) : base(XmlNodeType.Attribute, depth, name, value) { }

		public override XNode ToNode()
		{
			throw new NotSupportedException();
		}
	}

	public sealed class QElement : QXmlItem
	{
		public QElement(int depth, string name) : base(XmlNodeType.Element, depth, name, null) { }

		public override XNode ToNode()
		{
			return new XElement(Name);
		}
	}

	public sealed class QContent : QXmlItem
	{
		public QContent(int depth, string value) : base(XmlNodeType.Text, depth, null, value) { }

		public override XNode ToNode()
		{
			var x = new XElement("");

			return new XText(Value);
		}
	}

	public sealed class QData : QXmlItem
	{
		public QData(int depth, string value) : base(XmlNodeType.CDATA, depth, null, value) { }

		public override string Name
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override XNode ToNode()
		{
			return new XCData(Value);
		}
	}
}
