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

using System.Text;
using System.Threading;

namespace Randal.Core.IO.Logging
{
	public sealed class StringLogger : ILogger
	{
		public StringLogger(ILogEntryFormatter formatter = null)
		{
			_formatter = formatter ?? new LogEntryFormatter();
			_text = new StringBuilder();
			_lock = new ReaderWriterLockSlim();
			_verbosity = Verbosity.All;
		}

		public string GetLatestText()
		{
			string text;

			_lock.EnterWriteLock();
			try
			{
				text = _text.ToString();
				_text.Clear();
			}
			finally
			{
				_lock.ExitWriteLock();
			}

			return text;
		}

		public Verbosity VerbosityThreshold
		{
			get
			{
				_lock.EnterReadLock();
				try
				{
					return _verbosity;
				}
				finally
				{
					_lock.ExitReadLock();
				}
			}
		}

		public void ChangeVerbosityThreshold(Verbosity newLevel)
		{
			_lock.EnterWriteLock();
			try
			{
				_verbosity = newLevel;
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public void Add(ILogEntry entry)
		{
			_lock.EnterUpgradeableReadLock();
			try
			{
				if (entry.VerbosityLevel < _verbosity)
					return;

				_lock.EnterWriteLock();
				try
				{
					_text.Append(_formatter.Format(entry));
				}
				finally
				{
					_lock.ExitWriteLock();
				}
			}
			finally 
			{
				_lock.ExitUpgradeableReadLock();
			}
		}

		public void Dispose()
		{
			_text.Clear();
		}

		private readonly StringBuilder _text;
		private readonly ILogEntryFormatter _formatter;
		private volatile Verbosity _verbosity;
		private readonly ReaderWriterLockSlim _lock;
	}
}