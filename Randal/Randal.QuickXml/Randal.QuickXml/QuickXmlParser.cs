using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Randal.QuickXml
{
	internal enum QState
	{
		New
	}

	public interface IQuickXmlParser
	{
		XElement Parse(TextReader reader);
	}

	public class QuickXmlParser : IQuickXmlParser
	{
		public QuickXmlParser()
		{
			_state = QState.New;
		}

		public XElement Parse(TextReader reader)
		{


			return new XElement("Not implemented");
		}

		private QState _state;
	}
}
