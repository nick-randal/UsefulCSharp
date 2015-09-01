// Useful C#
// Copyright (C) 2014-2015 Nicholas Randal
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
	public static class LogEntryExtensions
	{
		public static ILogEntry ToLogEntry(this string message, Verbosity verbosity = Verbosity.Info)
		{
			return new LogEntry(message, verbosity);
		}

		public static ILogEntry ToLogEntry(this string message, DateTime timestamp, Verbosity verbosity = Verbosity.Info)
		{
			return new LogEntry(message, timestamp, verbosity);
		}

		public static ILogEntry ToLogEntryNoTs(this string message, Verbosity verbosity = Verbosity.Info)
		{
			return new LogEntryNoTimestamp(message, verbosity);
		}

		public static ILogEntry ToLogEntryException(this Exception ex, string message = null)
		{
			return new LogExceptionEntry(ex, message);
		}
	}
}