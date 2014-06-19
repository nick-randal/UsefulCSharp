﻿/*
Useful C#
Copyright (C) 2014  Nicholas Randal

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/

namespace Randal.Core.IO.Logging
{
	public sealed class NullLogger : ILogger
	{
		public Verbosity VerbosityThreshold
		{
			get { return Verbosity.All; }
		}

		public void ChangeVerbosityThreshold(Verbosity newLevel) { }

		public void Add(ILogEntry entry) { }

		public void Dispose() { }
	}
}
