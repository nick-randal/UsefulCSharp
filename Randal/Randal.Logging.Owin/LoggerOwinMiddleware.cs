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
using Microsoft.Owin;

namespace Randal.Logging.Owin
{
	public sealed class LoggerOwinMiddleware : OwinMiddleware, IDisposable
	{
		public LoggerOwinMiddleware(OwinMiddleware next, ILogSink logger) : this(next, logger, null)
		{
		}

		public LoggerOwinMiddleware(OwinMiddleware next, ILogSink logger, IOwinContextFormatter formatter)
			: base(next)
		{
			_logger = logger;
			_formatter = formatter ?? new OwinContextFormatter();
		}

		public override async Task Invoke(IOwinContext context)
		{
			if(_formatter.UsePreEntry)
				_logger.Post(_formatter.GetPreEntry(context));

			await Next.Invoke(context);

			if(_formatter.UsePostEntry)
				_logger.Post(_formatter.GetPostEntry(context));
		}

		public void Dispose()
		{
			_logger.Dispose();
		}

		private readonly ILogSink _logger;
		private readonly IOwinContextFormatter _formatter;
	}
}