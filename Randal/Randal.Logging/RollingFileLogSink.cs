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
	public sealed class RollingFileLogSink : IRollingFileLogSink
	{
		public RollingFileLogSink(IRollingFileSettings settings, IRollingFileManager logWriterManager = null, ILogEntryFormatter formatter = null, Verbosity verbosity = Verbosity.All)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			_settings = settings;
			_formatter = formatter ?? new LogEntryFormatter();
			_logWriterManager = logWriterManager ?? new RollingFileManager(settings, new LogFolder(settings.BasePath, settings.BaseFileName));

			_verbosity = verbosity;
		}

		public Verbosity VerbosityThreshold { get { return _verbosity; } }

		public string CurrentLogFilePath
		{
			get { return _logWriterManager.LogFileName; }
		}

		public void Post(ILogEntry entry)
		{
			if (entry.VerbosityLevel < VerbosityThreshold )
				return;

			if (_settings.ShouldTruncateRepeatingLines)
			{
				if (string.Compare(_lastRepeatedText, entry.Message, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					_repetitionCount++;
					return;
				}

				_lastRepeatedText = entry.Message;
				WriteRepetitionInfo();

				_repetitionCount = 1;
			}

			_logWriterManager.GetStreamWriter().Write(_formatter.Format(entry));
		}

		public void Dispose()
		{
			WriteRepetitionInfo();

			if (string.IsNullOrWhiteSpace(_settings.ClosingText) == false)
				_logWriterManager.GetStreamWriter().WriteLine(_settings.ClosingText);

			_logWriterManager.Dispose();
		}

		private void WriteRepetitionInfo()
		{
			if (_repetitionCount <= 1)
				return;

			_logWriterManager.GetStreamWriter().WriteLine(
				string.Concat(TextResources.AttentionRepeatingLinesPrefix, _repetitionCount, TextResources.AttentionRepeatingLinesSuffix)
			);
		}

		private readonly ILogEntryFormatter _formatter;
		private readonly IRollingFileManager _logWriterManager;
		private volatile Verbosity _verbosity;
		private readonly IRollingFileSettings _settings;
		private string _lastRepeatedText;
		private int _repetitionCount;
	}
}