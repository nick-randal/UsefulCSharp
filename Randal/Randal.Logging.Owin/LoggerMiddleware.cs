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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Randal.Logging.Owin
{
	using AppFunc = Func<IDictionary<string, object>, Task>;

	public sealed class LoggerMiddleware : IDisposable
	{
		public LoggerMiddleware(AppFunc next, ILogSink logger) : this(next, logger, null)
		{
		}

		public LoggerMiddleware(AppFunc next, ILogSink logger, IEnvironmentFormatter formatter)
		{
			_next = next;
			_logger = logger;
			_formatter = formatter ?? new EnvironmentFormatter();
		}

		public async Task Invoke(IDictionary<string, object> environment)
		{
			if (_formatter.UsePreEntry)
				_logger.Post(_formatter.GetPreEntry(environment));

			await _next.Invoke(environment);

			if (_formatter.UsePostEntry)
				_logger.Post(_formatter.GetPostEntry(environment));
		}

		public void Dispose()
		{
			_logger.Dispose();
		}

		private readonly ILogSink _logger;
		private readonly IEnvironmentFormatter _formatter;
		private readonly AppFunc _next;
	}
}