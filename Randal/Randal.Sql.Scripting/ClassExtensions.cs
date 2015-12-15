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

using System;
using Microsoft.SqlServer.Management.Smo;

namespace Randal.Sql.Scripting
{
	public static class DateTimeExtensions
	{
		public static string ToVersion(this DateTime date, int iteration)
		{
			if (iteration < 1)
				iteration = 1;

			if (iteration > 99)
				iteration = 99;

			return date.ToString(DateVersionFormat) + iteration.ToString("D2");
		}

		public const string
			DateVersionFormat = "yy.MM.dd."
		;
	}

	public static class IntegerExtensions
	{
		public static string ToVersionToday(this int iteration)
		{
			return DateTime.Today.ToVersion(iteration);
		}
	}

	public static class ScriptSchemaObjectBaseExtensions
	{
		public static string ScriptFileName(this ScriptSchemaObjectBase scriptObject)
		{
			return scriptObject.Schema.Replace('\\', '.') + '.' + scriptObject.Name;
		}
	}
}