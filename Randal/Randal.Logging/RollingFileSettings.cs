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

namespace Randal.Logging
{
	public sealed class RollingFileSettings : IRollingFileSettings
	{
		public RollingFileSettings(string basePath, string baseFileName, long fileSize = FiveMegabytes,
			bool truncateRepeatingLines = true, string closingText = "\r\n")
		{
			if (string.IsNullOrWhiteSpace(basePath))
				throw new ArgumentException("basePath");

			_fileSizeBytes = fileSize <= 0 ? FiveMegabytes : fileSize;

			_basePath = basePath;
			_baseFileName = baseFileName ?? string.Empty;
			_truncateRepeatingLines = truncateRepeatingLines;
			_closingText = closingText;
		}

		public long FileSize
		{
			get { return _fileSizeBytes; }
		}

		public string BasePath
		{
			get { return _basePath; }
		}

		public string BaseFileName
		{
			get { return _baseFileName; }
		}

		public bool ShouldTruncateRepeatingLines
		{
			get { return _truncateRepeatingLines; }
		}

		public string ClosingText
		{
			get { return _closingText; }
		}

		private readonly string _basePath, _baseFileName, _closingText;
		private readonly long _fileSizeBytes;
		private readonly bool _truncateRepeatingLines;

		public const long FiveMegabytes = 5242880;
	}
}