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
	public interface ILogEntry
	{
		bool ShowTimestamp { get; }
		DateTime Timestamp { get; }
		string Message { get; }
		Verbosity VerbosityLevel { get; }
	}

	public class LogEntry : ILogEntry
	{
		public LogEntry(string message, Verbosity verbosity = Verbosity.Info) : this(message, DateTime.Now, verbosity)
		{
		}

		public LogEntry(string message, DateTime timestamp, Verbosity verbosity = Verbosity.Info)
		{
			VerbosityLevel = verbosity;
			Timestamp = timestamp;
			Message = message ?? string.Empty;
		}

		public Verbosity VerbosityLevel { get; protected set; }

		public virtual bool ShowTimestamp
		{
			get { return true; }
		}

		public string Message { get; protected set; }

		public DateTime Timestamp { get; protected set; }
	}
}