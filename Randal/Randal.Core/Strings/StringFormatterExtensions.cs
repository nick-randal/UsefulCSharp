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

namespace Randal.Core.Strings
{
	public static class StringFormatterExtensions
	{
		internal static IStringFormatter Formatter = new NamedFieldFormatter();
		internal static Func<string, IStringFormatHelper> Factory = target => new StringFormatHelper(target, Formatter);

		public static IStringFormatHelper Format(this string target)
		{
			return Factory.Invoke(target);
		}
	}
}