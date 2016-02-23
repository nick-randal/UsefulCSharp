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

using System.IO;
using System.Text.RegularExpressions;

namespace Randal.Core.T4
{
	public static class StringExtensions
	{
		public static string ToCSharpPropertyName(this string text)
		{
			var temp = PropertyNameRegex.Replace(text, CleanupMatch);

			if(temp.Length == 0)
				throw new InvalidDataException("Incoming value did not contain any valid characters.");

			if (char.IsDigit(temp[0]) == false)
				return temp;

			temp = PropertyNumbers[temp[0] - 48] + temp.Substring(1);

			return temp;
		}

		private static string CleanupMatch(Match match)
		{
			if(match.Groups["a"].Success)
				return match.Groups["a"].Value.ToUpper();

			return match.Groups["b"].Success ? match.Groups["b"].Value : string.Empty;
		}

		private const RegexOptions StandardOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | 
			RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture;
		private static readonly Regex PropertyNameRegex = new Regex(@"(?<a>^[a-z0-9]) | ([^a-z0-9](?<a>[a-z0-9])) | (?<b>[a-z0-9]) | (?<c>\W)", StandardOptions);
		private static readonly string[] PropertyNumbers =
		{
			"Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine"
		};
	}
}