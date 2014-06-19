/*
Useful C#
Copyright (C) 2014  Nicholas Randal

Useful C# is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/

using System;

namespace Randal.Core.IO.Logging
{
	public sealed class LoggerStringFormatDecorator : ILogger
	{
		public LoggerStringFormatDecorator(ILogger logger)
		{
			_logger = logger;
		}

		public void AddEntry(string message, params object[] values)
		{
			AddEntry(Verbosity.Info, message, values);
		}

		public void AddEntry(Verbosity verbosity, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			_logger.Add(new LogEntry(formatted, verbosity));
		}

		public void AddGroup(string message, params object[] values)
		{
			AddGroup(Verbosity.Info, message, values);
		}

		public void AddGroup(Verbosity verbosity, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			_logger.Add(new LogGroupEntry(_logger, formatted, verbosity));
		}

		public void AddEntryNoTimestamp(string message, params object[] values)
		{
			AddEntryNoTimestamp(Verbosity.Info, message, values);
		}

		public void AddEntryNoTimestamp(Verbosity verbosity, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			_logger.Add(new LogEntryNoTimestamp(formatted, verbosity));
		}

		public void AddException(Exception ex, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			_logger.Add(new ExceptionEntry(ex, formatted));
		}

		#region ILogger Wrappers

		public Verbosity VerbosityThreshold
		{
			get { return _logger.VerbosityThreshold; }
		}

		public void ChangeVerbosityThreshold(Verbosity newLevel)
		{
			_logger.ChangeVerbosityThreshold(newLevel);
		}

		public void Add(ILogEntry entry)
		{
			_logger.Add(entry);
		}

		public void Dispose()
		{
			_logger.Dispose();
		}

		#endregion

		private readonly ILogger _logger;
	}
}