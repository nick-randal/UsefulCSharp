﻿// Useful C#
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
	public interface ILoggerStringFormatDecorator : ILogger
	{
		void AddEntry(string message, params object[] values);
		void AddEntry(Verbosity verbosity, string message, params object[] values);
		void AddBlank(Verbosity verbosity = Verbosity.Info);
		void AddEntryNoTimestamp(string message, params object[] values);
		void AddEntryNoTimestamp(Verbosity verbosity, string message, params object[] values);
		void AddException(Exception ex);
		void AddException(Exception ex, string message, params object[] values);

		ILogger Logger { get; }
	}

	public sealed class LoggerStringFormatDecorator : ILoggerStringFormatDecorator
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

		public void AddBlank(Verbosity verbosity = Verbosity.Info)
		{
			_logger.Add(new LogEntryNoTimestamp(string.Empty, verbosity));
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

		public void AddException(Exception ex)
		{
			_logger.Add(new ExceptionEntry(ex));
		}

		public void AddException(Exception ex, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			_logger.Add(new ExceptionEntry(ex, formatted));
		}

		public ILogger Logger
		{
			get { return _logger; }
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