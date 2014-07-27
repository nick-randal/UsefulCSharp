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

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Randal.Logging
{
	public sealed class AsyncFileLogger : ILogger
	{
		public AsyncFileLogger(IFileLoggerSettings settings, ILogFileManager logWriterManager = null,
			ILogEntryFormatter formatter = null)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			_formatter = formatter ?? new LogEntryFormatter();
			_logWriterManager = logWriterManager ??
			                    new LogFileManager(settings, new LogFolder(settings.BasePath, settings.BaseFileName));

			_logQueue = new ConcurrentQueue<ILogEntry>();
			_signal = new ManualResetEventSlim(false);
			_token = new CancellationTokenSource();
			_settingsLock = new ReaderWriterLockSlim();

			_task = Task.Run(() => RunAsync(settings), CancellationToken);
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

		private CancellationToken CancellationToken
		{
			get { return _token.Token; }
		}

		private void RequestCancellation()
		{
			_token.Cancel();
		}

		public void Dispose()
		{
			if (_disposed)
				return;

			RequestCancellation();

			try
			{
				DisposeOfTask();

				DisposeOfManualEvent();

				DisposeOfCancellationToken();

				DisposeOfReaderWriterLock();
			}
			catch (Exception ex)
			{
				EventLog.WriteEntry(TextResources.ApplicationEventLog, ex.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				_disposed = true;
			}
		}

		private void DisposeOfReaderWriterLock()
		{
			if (_settingsLock == null)
				return;

			try
			{
				_settingsLock.Dispose();
			}
			finally
			{
				_settingsLock = null;
			}
		}

		private void DisposeOfCancellationToken()
		{
			if (_token == null)
				return;

			try
			{
				_token.Dispose();
			}
			finally
			{
				_token = null;
			}
		}

		private void DisposeOfManualEvent()
		{
			if (_signal == null)
				return;

			try
			{
				_signal.Dispose();
			}
			finally
			{
				_signal = null;
			}
		}

		private void DisposeOfTask()
		{
			if (_task == null)
				return;

			try
			{
				_task.Wait(5000);
				_task.Dispose();
			}
			finally
			{
				_task = null;
			}
		}

		private bool HasEntriesAndIsNotCancelled(out ILogEntry entry)
		{
			try
			{
				if (_logQueue.TryDequeue(out entry))
					return true;

				_signal.Reset();
				_signal.Wait(_token.Token);
				_logQueue.TryDequeue(out entry);
			}
			catch (OperationCanceledException)
			{
				entry = null;
				return false;
			}

			return true;
		}

		private async Task RunAsync(IFileLoggerSettings settings)
		{
			try
			{
				var repetitionCount = await ProcessEntriesAsync(settings.ShouldTruncateRepeatingLines);

				await WriteRepetitionInfo(repetitionCount);

				await _logWriterManager.GetStreamWriter().WriteLineAsync();
			}
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

		private async Task<int> ProcessEntriesAsync(bool truncateRepetition)
		{
			string lastRepeatedText = null;
			var repetitionCount = 1;
			ILogEntry entry;

			while (HasEntriesAndIsNotCancelled(out entry))
			{
				if (truncateRepetition)
				{
					if (string.Compare(lastRepeatedText, entry.Message, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						repetitionCount++;
						continue;
					}

					lastRepeatedText = entry.Message;
					await WriteRepetitionInfo(repetitionCount);

					repetitionCount = 1;
				}

				await _logWriterManager.GetStreamWriter().WriteAsync(_formatter.Format(entry));
			}

			return repetitionCount;
		}

		private async Task WriteRepetitionInfo(int repetitionCount)
		{
			if (repetitionCount <= 1)
				return;

			await _logWriterManager.GetStreamWriter().WriteLineAsync(
				string.Concat(TextResources.AttentionRepeatingLinesPrefix, repetitionCount, TextResources.AttentionRepeatingLinesSuffix)
			);
		}

		internal const int SetupWait = 1000;

		private volatile bool _disposed;
		private Task _task;
		private ManualResetEventSlim _signal;
		private ReaderWriterLockSlim _settingsLock;
		private CancellationTokenSource _token;
		private readonly ConcurrentQueue<ILogEntry> _logQueue;
		private readonly ILogEntryFormatter _formatter;
		private readonly ILogFileManager _logWriterManager;
		private volatile Verbosity _verbosity;
	}
}