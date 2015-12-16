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
	public interface ILoggerAsync
	{
		Task PostEntryAsync(string message, params object[] values);

		Task PostEntryAsync(Verbosity verbosity, string message, params object[] values);

		Task PostBlankAsync(Verbosity verbosity = Verbosity.Info);

		Task PostEntryNoTimestampAsync(string message, params object[] values);

		Task PostEntryNoTimestampAsync(Verbosity verbosity, string message, params object[] values);

		Task PostExceptionAsync(Exception ex);

		Task PostExceptionAsync(Exception ex, string message, params object[] values);
	}
}