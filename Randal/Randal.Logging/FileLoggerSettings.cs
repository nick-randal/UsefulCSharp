// Useful C#
// Copyright (C) 2014 Nicholas Randal
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
	public interface IFileLoggerSettings
	{
		long FileSize { get; }
		string BasePath { get; }
		string BaseFileName { get; }
		bool ShouldTruncateRepeatingLines { get; }
	}

	public sealed class FileLoggerSettings : IFileLoggerSettings
	{
		public FileLoggerSettings(string basePath, string baseFileName, long fileSize = FiveMegabytes,
			bool truncateRepeatingLines = true)
		{
			if (string.IsNullOrWhiteSpace(basePath))
				throw new ArgumentException("basePath");

			_fileSizeBytes = fileSize <= 0 ? FiveMegabytes : fileSize;

			_basePath = basePath;
			_baseFileName = baseFileName ?? string.Empty;
			_truncateRepeatingLines = truncateRepeatingLines;
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

		private readonly string _basePath, _baseFileName;
		private readonly long _fileSizeBytes;
		private readonly bool _truncateRepeatingLines;

		public const long FiveMegabytes = 5242880;
	}
}