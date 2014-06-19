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
	public interface ILogGroupEntry : ILogEntry
	{
		bool IsEnd { get; }
	}

	public sealed class LogGroupEntry : LogEntry, ILogGroupEntry, IDisposable
	{
		public LogGroupEntry(ILogger logger, string groupName, Verbosity verbosity = Verbosity.Info) : base(groupName, DateTime.MinValue, verbosity)
		{
			_logger = logger;
			IsEnd = false;
		}

		public override bool ShowTimestamp
		{
			get { return false; }
		}

		public void Dispose()
		{
			if (_logger == null) 
				return;

			var groupEnd = new LogGroupEntry(null, Message, VerbosityLevel)
			{
				IsEnd = true
			};
			_logger.Add(groupEnd);
		}

		public bool IsEnd { get; private set; }

		private readonly ILogger _logger;
	}
}