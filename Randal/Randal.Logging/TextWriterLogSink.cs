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
using System.IO;
using System.Threading;

namespace Randal.Logging
{
	public class TextWriterLogSink : ILogSink
	{
		public TextWriterLogSink(Stream stream, ILogEntryFormatter formatter = null)
			: this(new StreamWriter(stream), formatter)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
		}

		public TextWriterLogSink(TextWriter writer, ILogEntryFormatter formatter = null)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			Writer = writer;
			Lock = new ReaderWriterLockSlim();
			Formatter = formatter ?? new LogEntryFormatter();
		}

		public void Post(ILogEntry entry)
		{
			Lock.EnterUpgradeableReadLock();
			try
			{
				if (entry.VerbosityLevel < Verbosity)
					return;

				Lock.EnterWriteLock();
				try
				{
					Writer.Write(Formatter.Format(entry));
				}
				finally
				{
					Lock.ExitWriteLock();
				}
			}
			finally
			{
				Lock.ExitUpgradeableReadLock();
			}
		}

		public Verbosity VerbosityThreshold
		{
			get
			{
				Lock.EnterReadLock();
				try
				{
					return Verbosity;
				}
				finally
				{
					Lock.ExitReadLock();
				}
			}
		}

		public void ChangeVerbosityThreshold(Verbosity newLevel)
		{
			Lock.EnterWriteLock();
			try
			{
				Verbosity = newLevel;
			}
			finally
			{
				Lock.ExitWriteLock();
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Disposed)
				return;

			Disposed = true;

			if (!disposing) 
				return;

			Writer.Flush();
			Writer.Close();
			Writer.Dispose();

			Lock.Dispose();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~TextWriterLogSink()
		{
			Dispose(false);
		}

		protected bool Disposed = false;
		protected readonly TextWriter Writer;
		protected readonly ILogEntryFormatter Formatter;
		protected volatile Verbosity Verbosity;
		protected readonly ReaderWriterLockSlim Lock;
	}
}