/*
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

using System.Threading;

namespace Randal.Core.IO.Logging
{
	public sealed class OneLog
	{
		private static readonly ReaderWriterLockSlim InstLock = new ReaderWriterLockSlim();
		private static ILogger _instance;

		static OneLog()
		{
			Inst = new StringLogger();
		}

		public static ILogger Inst
		{
			get
			{
				InstLock.EnterReadLock();
				try
				{
					return _instance;
				}
				finally
				{
					InstLock.ExitReadLock();
				}
			}
			set
			{
				InstLock.EnterWriteLock();
				try
				{
					_instance = value;
				}
				finally
				{
					InstLock.ExitWriteLock();
				}
			}
		}
	}
}