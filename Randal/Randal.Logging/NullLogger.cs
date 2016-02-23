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
using System.Threading.Tasks;

namespace Randal.Logging
{
	public sealed class NullLogger : ILogger
	{
		public void AddLogSink(ILogSink logSink) { }

		public Task CompleteAllAsync(int attemptsToComplete = 3, TimeSpan? delayBetweenAttempts = null)
		{
			return Task.FromResult(false);
		}

		public Task PostEntryAsync(string message, params object[] values)
		{
			return Task.FromResult(false);
		}

		public void PostEntry(string message, params object[] values) { }

		public Task PostEntryAsync(Verbosity verbosity, string message, params object[] values)
		{
			return Task.FromResult(false);
		}

		public void PostEntry(Verbosity verbosity, string message, params object[] values) { }

		public Task PostBlankAsync(Verbosity verbosity = Verbosity.Info)
		{
			return Task.FromResult(false);
		}

		public void PostBlank(Verbosity verbosity = Verbosity.Info) { }

		public Task PostEntryNoTimestampAsync(string message, params object[] values)
		{
			return Task.FromResult(false);
		}

		public void PostEntryNoTimestamp(string message, params object[] values) { }

		public Task PostEntryNoTimestampAsync(Verbosity verbosity, string message, params object[] values)
		{
			return Task.FromResult(false);
		}

		public void PostEntryNoTimestamp(Verbosity verbosity, string message, params object[] values) { }

		public Task PostExceptionAsync(Exception ex)
		{
			return Task.FromResult(false);
		}

		public void PostException(Exception ex) { }

		public Task PostExceptionAsync(Exception ex, string message, params object[] values)
		{
			return Task.FromResult(false);
		}

		public void PostException(Exception ex, string message, params object[] values) { }
	}
}
