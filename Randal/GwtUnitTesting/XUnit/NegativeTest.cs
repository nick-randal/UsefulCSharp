// Useful C#
// Copyright (C) 2014-2019 Nicholas Randal
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
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GwtUnit.XUnit
{
	[TraitDiscoverer("GwtUnit.XUnit.NegativeTestDiscoverer", "GwtUnit")]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class NegativeTestAttribute : Attribute, ITraitAttribute
	{
	}

	public class NegativeTestDiscoverer : ITraitDiscoverer
	{
		public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
		{
			yield return new KeyValuePair<string, string>("Category", "Negative");
		}
	}
}