// Useful C#
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

using Microsoft.Owin;

namespace Randal.Logging.Owin
{
	public interface ILoggerOwinFormatter
	{
		bool UsePreEntry { get; }
		ILogEntry GetPreEntry(IOwinContext context);
		bool UsePostEntry { get; }
		ILogEntry GetPostEntry(IOwinContext context);
	}

	public sealed class LoggerOwinFormatter : ILoggerOwinFormatter
	{
		public bool UsePreEntry
		{
			get { return true; }
		}

		public ILogEntry GetPreEntry(IOwinContext context)
		{
			return new LogEntry(" in  " + context.Environment["owin.RequestPath"]);
		}

		public bool UsePostEntry
		{
			get { return true; }
		}

		public ILogEntry GetPostEntry(IOwinContext context)
		{
			return new LogEntry("out  " + context.Environment["owin.RequestPath"]);
		}
	}
}