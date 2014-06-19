/*
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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using Randal.Core.IO.Logging.FileHandling;

namespace Randal.Core.IO.Logging
{
	public enum AsyncFileLoggerState
	{
		Created,
		Running,
		Disposed
	}

	public sealed class AsyncFileLogger : ILogger
	{
		public AsyncFileLogger(IFileLoggerSettings settings, ILogFileManager logWriterManager = null, ILogEntryFormatter formatter = null)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			_formatter = formatter ?? new LogEntryFormatter();
			_logWriterManager = logWriterManager ?? new LogFileManager(settings, new LogFolder(settings.BasePath, settings.BaseFileName));

			_logQueue = new ConcurrentQueue<ILogEntry>();
			_signal = new ManualResetEventSlim(false);
			_token = new CancellationTokenSource();
			_settingsLock = new ReaderWriterLockSlim();
			_task = Task.Factory.StartNew(Run, settings, CancellationToken);
		}

		public Verbosity VerbosityThreshold
		{
			get
			{
				_settingsLock.EnterReadLock();
				try
				{
					return _verbosity;
				}
				finally
				{
					_settingsLock.ExitReadLock();
				}
			}
		}

		public AsyncFileLoggerState State
		{
			get
			{
				_settingsLock.EnterReadLock();
				try
				{
					return _state;
				}
				finally
				{
					_settingsLock.ExitReadLock();
				}
			}

			set
			{
				_settingsLock.EnterWriteLock();
				try
				{
					_state = value;
				}
				finally
				{
					_settingsLock.ExitWriteLock();
				}
			}
		}

		public void ChangeVerbosityThreshold(Verbosity newLevel)
		{
			_settingsLock.EnterWriteLock();
			try
			{
				_verbosity = newLevel;
			}
			finally
			{
				_settingsLock.ExitWriteLock();
			}
		}

		public void Add(ILogEntry entry)
		{
			if (_token.IsCancellationRequested)
				return;

			_logQueue.Enqueue(entry);
			_signal.Set();
		}

		private CancellationToken CancellationToken { get { return _token.Token; } }

		private void RequestCancellation()
		{
			_token.Cancel();
		}

		private bool WaitForEntries()
		{
			try
			{
				_signal.Wait(_token.Token);
				return false;
			}
			catch (OperationCanceledException)
			{
				return true;
			}
		}

		private bool GetNextEntryOrReset(out ILogEntry entry)
		{
			var result = _logQueue.TryDequeue(out entry);

			if (result == false)
				_signal.Reset();

			return result;
		}

		public void Dispose()
		{
			State = AsyncFileLoggerState.Disposed;
			// allow a few more messages before shutting down
			Thread.Sleep(500);

			RequestCancellation();

			try
			{
				if (_task == null) 
					return;

				for (var n = 0; n < 40; n++)	// wait up to 10 seconds
				{
					if (_task.Status == TaskStatus.RanToCompletion ||
					    _task.Status == TaskStatus.Faulted ||
					    _task.Status == TaskStatus.Canceled)
						break;

					Thread.Sleep(250);
				}

				if (_task.IsCompleted || _task.IsCanceled || _task.IsFaulted)
					_task.Dispose();
				_task = null;
			}
			catch(Exception ex) 
			{
				EventLog.WriteEntry(TextResources.ApplicationEventLog, ex.ToString(), EventLogEntryType.Error);
			}
		}

		private void Run(object asyncState)
		{
			string lastRepeatedText = null;
			var repetitionCount = 1;

			State = AsyncFileLoggerState.Running;

			try
			{
				var settings = (FileLoggerSettings)asyncState;

				while (true)
				{
					// block until entries are available, if we get cancelled then return
					// the base class, signals, waits and then cancels, so pending entries should get written
					if (WaitForEntries())
					{
						WriteRepetitionInfo(repetitionCount);
						return;
					}

					ILogEntry entry;
					if (GetNextEntryOrReset(out entry) == false)
						continue;

					if (settings.ShouldTruncateRepeatingLines)
					{
						if (string.Compare(lastRepeatedText, entry.Message, StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							repetitionCount++;
							continue;
						}

						lastRepeatedText = entry.Message;
						WriteRepetitionInfo(repetitionCount);

						repetitionCount = 1;
					}

					// Get the current stream or a new stream if necessary
					var writer = _logWriterManager.GetStreamWriter();
					writer.Write(_formatter.Format(entry));
				}
			}
				// generally we do not catch exceptions like this, safe guard against the thread death and not being cleaned up appropriately
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				EventLog.WriteEntry(TextResources.ApplicationEventLog, ex.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				_logWriterManager.Dispose();
			}
		}

		private void WriteRepetitionInfo(int repetitionCount)
		{
			if (repetitionCount <= 1)
				return;

			var writer = _logWriterManager.GetStreamWriter();
			writer.WriteLine(TextResources.AttentionRepeatingLines, repetitionCount);
		}

		internal const int SetupWait = 1000;

		private Task _task;
		private AsyncFileLoggerState _state = AsyncFileLoggerState.Created;
		private readonly CancellationTokenSource _token;
		private readonly ConcurrentQueue<ILogEntry> _logQueue;
		private readonly ManualResetEventSlim _signal;
		private readonly ILogEntryFormatter _formatter;
		private readonly ILogFileManager _logWriterManager;
		private readonly ReaderWriterLockSlim _settingsLock;
		private volatile Verbosity _verbosity;
	}
}