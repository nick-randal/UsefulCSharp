﻿/*
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
	public interface ILogEntryFormatter
	{
		string Format(ILogEntry entry);
	}

	public sealed class LogEntryFormatter : ILogEntryFormatter
	{
		public string Format(ILogEntry entry)
		{
			return string.Concat(
						entry.ShowTimestamp ? entry.Timestamp.ToString(TextResources.Timestamp) : TextResources.NoTimestamp,
						' ',
						entry.Message,
						Environment.NewLine
					);
		}
	}
}