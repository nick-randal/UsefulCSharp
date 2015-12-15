// Useful C#
// Copyright (C) 2014-2015 Nicholas Randal
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
using System.Data;
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

			_queuedEntries = new BlockingCollection<ILogEntry>();
			_tokenSource = new CancellationTokenSource();
			_settingsLock = new ReaderWriterLockSlim();
			_verbosity = Verbosity.All;

			_task = Task.Run(() => RunAsync(settings), _tokenSource.Token);
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

		public string CurrentLogFilePath
		{
			get { return _logWriterManager.LogFileName; }
		}

		public void Add(ILogEntry entry)
		{
			if (_tokenSource.IsCancellationRequested || _queuedEntries.IsAddingCompleted || entry.VerbosityLevel < VerbosityThreshold )
				return;

			_queuedEntries.TryAdd(entry, Timeout.Infinite, _tokenSource.Token);
		}

		public void Dispose()
		{
			lock (_queuedEntries)
			{	
				if (_disposed)
					return;

				_queuedEntries.CompleteAdding();
				Monitor.Wait(_queuedEntries, 2500);
				_tokenSource.Cancel();

				try
				{
					DisposeOfTask();

					DisposeOfCancellationToken();

					DisposeOfReaderWriterLock();

					_queuedEntries.Dispose();
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
			if (_tokenSource == null)
				return;

			try
			{
				_tokenSource.Dispose();
			}
			finally
			{
				_tokenSource = null;
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

		/// <summary>
		/// Wraps the entire Queue process so that if the final log entries are repeats and have not been written to the log, the log can output that final entry.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		private async Task RunAsync(IFileLoggerSettings settings)
		{
			try
			{
				var repetitionCount = await ProcessEntriesAsync(settings.ShouldTruncateRepeatingLines);

				await WriteRepetitionInfo(repetitionCount);

				if (string.IsNullOrEmpty(settings.ClosingText))
					return;

				await _logWriterManager.GetStreamWriter().WriteLineAsync(settings.ClosingText);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				EventLog.WriteEntry(TextResources.ApplicationEventLog, ex.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				lock(_queuedEntries)
					Monitor.Pulse(_queuedEntries);
				_logWriterManager.Dispose();
			}
		}

		private async Task<int> ProcessEntriesAsync(bool truncateRepetition)
		{
			string lastRepeatedText = null;
			var repetitionCount = 1;

			while (true)
			{
				ILogEntry entry;

				try
				{
					if (_queuedEntries.TryTake(out entry, Timeout.Infinite, _tokenSource.Token) == false || entry == null)
						return repetitionCount;
				}
				catch (OperationAbortedException)
				{
					return 0;
				}

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
		private ReaderWriterLockSlim _settingsLock;
		private CancellationTokenSource _tokenSource;
		private readonly BlockingCollection<ILogEntry> _queuedEntries;
		private readonly ILogEntryFormatter _formatter;
		private readonly ILogFileManager _logWriterManager;
		private volatile Verbosity _verbosity;
	}
}