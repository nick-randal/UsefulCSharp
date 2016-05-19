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

using System;

namespace Randal.Core.T4
{
	public sealed class DbCodeDefinition
	{
		private readonly object _code, _name, _displayName, _description, _obsolete;

		public DbCodeDefinition(object code, object name, object displayName, object description, object obsolete = null)
		{
			_code = code;
			_name = name;
			_displayName = displayName;
			_description = description;
			_obsolete = obsolete ?? false;
		}

		public string Code => _code?.ToString() ?? string.Empty;

		public string Name => _name?.ToString() ?? string.Empty;

		public string DisplayName => _displayName?.ToString() ?? string.Empty;

		public string NameAsCSharpProperty => Name?.ToCSharpPropertyName() ?? string.Empty;

		public string Description => _description?.ToString() ?? string.Empty;

		public bool IsObsolete => Convert.ToBoolean(_obsolete);
	}
}