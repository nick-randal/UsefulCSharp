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

namespace Randal.Logging
{
	public sealed class RollingFileManager : IRollingFileManager
	{
		public RollingFileManager(IRollingFileSettings settings, ILogFolder logFolder = null)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			_settings = settings;
			_logFolder = logFolder ?? new LogFolder(settings.BasePath, settings.BaseFileName);
			_sync = new object();
		}

		public StreamWriter GetStreamWriter()
		{
			lock (_sync)
			{
				StreamWriter streamWriter = null;
				var attempts = 0;
				var lastLogFile = _currentLogFile;

				for (; attempts < MaxAttempts; attempts++)
				{
					_currentLogFile = CreateOrOpenLogFile(_currentLogFile, _logFolder.GetNextLogFilePath);
					if (_currentLogFile.State == LogFileState.OpenAvailable)
					{
						streamWriter = _currentLogFile.GetStreamWriter();
						break;
					}

					if (lastLogFile != null && lastLogFile != _currentLogFile)
						CloseLogFile(lastLogFile);

					lastLogFile = _currentLogFile;
					_currentLogFile = null;
				}

				if (streamWriter == null)
					throw new InvalidOperationException("Could not obtain a valid log file instance after " + attempts + " attempts.");

				if (lastLogFile == null || lastLogFile == _currentLogFile)
					return streamWriter;

				if (lastLogFile.State == LogFileState.OpenAvailable || lastLogFile.State == LogFileState.OpenExhausted)
					lastLogFile.GetStreamWriter().WriteLine(CutoverToFormat, Path.GetFileName(_currentLogFile.FilePath));

				streamWriter.WriteLine(CutoverFromFormat, Path.GetFileName(lastLogFile.FilePath));
				CloseLogFile(lastLogFile);

				return streamWriter;
			}
		}

		private static void CloseLogFile(ILogFile logFile)
		{
			if (logFile == null)
				return;

			if (logFile.State != LogFileState.Disposed)
				logFile.Dispose();
		}

		private ILogFile CreateOrOpenLogFile(ILogFile logFile, Func<string> getFilPath)
		{
			logFile = logFile ?? new LogFile(getFilPath(), _settings.FileSize);
			var state = logFile.State;

			if (state == LogFileState.Disposed)
				return logFile;

			if (state == LogFileState.Unknown)
				logFile.Open();

			return logFile;
		}

		public string LogFileName
		{
			get
			{
				lock (_sync)
				{
					return _currentLogFile == null ? null : _currentLogFile.FilePath;
				}
			}
		}

		public void Dispose()
		{
			if (_currentLogFile == null)
				return;

			_currentLogFile.Dispose();
			_currentLogFile = null;
		}

		private readonly IRollingFileSettings _settings;
		private readonly ILogFolder _logFolder;
		private ILogFile _currentLogFile;
		private readonly object _sync;

		private const int MaxAttempts = 3;

		private const string
			CutoverToFormat = "ATTENTION: Cutover to {0}",
			CutoverFromFormat = "ATTENTION: Cutover from {0}"
			;
	}
}