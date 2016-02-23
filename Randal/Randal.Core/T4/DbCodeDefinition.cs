// Useful C#
// Copyright (C) 2015-2016 Nicholas Randal
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

namespace Randal.Core.T4
{
	public sealed class DbCodeDefinition
	{
		private readonly object _code, _name, _displayName, _description;

		public DbCodeDefinition(object code, object name, object displayName, object description)
		{
			_code = code;
			_name = name;
			_displayName = displayName;
			_description = description;
		}

		public string Code
		{
			get { return _code.ToString(); }
		}

		public string Name
		{
			get { return _name.ToString(); }
		}

		public string DisplayName
		{
			get { return _displayName.ToString(); }
		}

		public string NameAsCSharpProperty
		{
			get { return Name.ToCSharpPropertyName(); }
		}

		public string Description
		{
			get { return _description.ToString(); }
		}
	}
}