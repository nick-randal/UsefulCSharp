// // Useful C#
// // Copyright (C) 2014 Nicholas Randal
// // 
// // Useful C# is free software; you can redistribute it and/or modify
// // it under the terms of the GNU General Public License as published by
// // the Free Software Foundation; either version 2 of the License, or
// // (at your option) any later version.
// // 
// // This program is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// // GNU General Public License for more details.

using System;
using System.Xml;

namespace Randal.QuickXml
{
	public sealed class QElement : QuickXmlItem
	{
		public QElement(int depth, string name) : base(XmlNodeType.Element, depth)
		{
			Name = name;
		}

		public override string Value
		{
			get { throw new NotSupportedException(); }
		}
	}
}