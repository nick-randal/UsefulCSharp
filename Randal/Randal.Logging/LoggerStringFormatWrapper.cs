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
	public interface ILoggerStringFormatWrapper
	{
		void AddEntry(string message, params object[] values);
		void AddEntry(Verbosity verbosity, string message, params object[] values);
		void AddBlank(Verbosity verbosity = Verbosity.Info);
		void AddEntryNoTimestamp(string message, params object[] values);
		void AddEntryNoTimestamp(Verbosity verbosity, string message, params object[] values);
		void AddException(Exception ex);
		void AddException(Exception ex, string message, params object[] values);
		ILogger BaseLogger { get; }
	}

	public class LoggerStringFormatWrapper : ILoggerStringFormatWrapper
	{
		public LoggerStringFormatWrapper(ILogger logger)
		{
			Logger = logger;
		}

		public void AddEntry(string message, params object[] values)
		{
			AddEntry(Verbosity.Info, message, values);
		}

		public void AddEntry(Verbosity verbosity, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			Logger.Add(new LogEntry(formatted, verbosity));
		}

		public void AddBlank(Verbosity verbosity = Verbosity.Info)
		{
			Logger.Add(new LogEntryNoTimestamp(string.Empty, verbosity));
		}

		public void AddEntryNoTimestamp(string message, params object[] values)
		{
			AddEntryNoTimestamp(Verbosity.Info, message, values);
		}

		public void AddEntryNoTimestamp(Verbosity verbosity, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			Logger.Add(new LogEntryNoTimestamp(formatted, verbosity));
		}

		public void AddException(Exception ex)
		{
			Logger.Add(new LogExceptionEntry(ex));
		}

		public void AddException(Exception ex, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			Logger.Add(new LogExceptionEntry(ex, formatted));
		}

		public ILogger BaseLogger
		{
			get { return Logger; }
		}

		protected readonly ILogger Logger;
	}
}