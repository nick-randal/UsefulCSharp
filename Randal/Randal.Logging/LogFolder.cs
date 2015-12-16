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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Randal.Logging
{
	public sealed class LogFolder : ILogFolder
	{
		public LogFolder(string path, string baseLogFileName)
		{
			_baseLogFileName = baseLogFileName;

			_directory = new DirectoryInfo(path);

			var pattern = Regex.Escape(baseLogFileName) + LogFileSuffixPattern;
			_indexedLogFile = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			_lastAttemptedIndex = -1;
		}

		public bool VerifyOrCreate()
		{
			try
			{
				if (_directory.Exists == false)
					_directory.Create();

				return true;
			}
			catch
			{
				return false;
			}
		}

		public string GetNextLogFilePath()
		{
			if (_lastAttemptedIndex < 1)
				_lastAttemptedIndex = FindLastUsedIndex();
			else
				_lastAttemptedIndex++;

			return Path.Combine(_directory.FullName, string.Format(StandardFilename, _baseLogFileName, _lastAttemptedIndex));
		}

		private int FindLastUsedIndex()
		{
			var files = _directory.GetFiles(FindExtension, SearchOption.TopDirectoryOnly);

			var ndx = files
				.Select(f =>
				{
					int n;

					var match = _indexedLogFile.Match(f.Name);
					if (match.Success && int.TryParse(match.Groups[1].Value, out n))
						return n;

					return -1;
				})
				.OrderByDescending(n => n)
				.FirstOrDefault();

			return ndx <= 0 ? 1 : ndx;
		}

		public string GetFallbackFilePath()
		{
			return Path.Combine(_directory.FullName, string.Format(FailFilename, _baseLogFileName, Guid.NewGuid()));
		}

		private int _lastAttemptedIndex;
		private readonly string _baseLogFileName;
		private readonly DirectoryInfo _directory;
		private readonly Regex _indexedLogFile;

		public const string
			LogFileSuffixPattern = @"_(\d+)\.log$",
			StandardFilename = "{0}_{1:000}.log",
			FailFilename = "{0}_{1}.log",
			FindExtension = "*.log"
			;
	}
}