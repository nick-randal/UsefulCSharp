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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Randal.Logging
{
	public sealed class Logger : ILogger, IDisposable
	{
		public Logger(int maxBufferCapacity = 100, params ILogSink[] sinks)
		{
			_cancellationSource = new CancellationTokenSource();

			_buffer = new BufferBlock<ILogEntry>(
				new DataflowBlockOptions
				{
					BoundedCapacity = maxBufferCapacity,
					CancellationToken = _cancellationSource.Token
				});

			_broadcast = new BroadcastBlock<ILogEntry>(entry => entry, new DataflowBlockOptions { CancellationToken = _cancellationSource.Token });

			LinkDataflowBlocks(_buffer, _broadcast);

			_sinks = new List<ActionBlock<ILogEntry>>();

			foreach(var sink in sinks)
				AddLogSink(sink);
		}

		private static void LinkDataflowBlocks<TBlock>(ISourceBlock<TBlock> source, ITargetBlock<TBlock> target)
		{
			source.LinkTo(target);
			source.Completion.ContinueWith(t =>
			{
				if (t.IsFaulted) 
					target.Fault(t.Exception);
				else 
					target.Complete();
			});
		}

		public void AddLogSink(ILogSink logSink)
		{
			var actionBlock = new ActionBlock<ILogEntry>(entry => logSink.Post(entry), new ExecutionDataflowBlockOptions { CancellationToken = _cancellationSource.Token });
			_sinks.Add(actionBlock);

			LinkDataflowBlocks(_broadcast, actionBlock);
		}

		public async Task CompleteAllAsync(int attemptsToComplete = 3, TimeSpan? delayBetweenAttempts = null)
		{
			try
			{
				for (var attempt = 0; _buffer.Count > 0 && attempt < attemptsToComplete; attempt++)
				{
					await Task.Delay(delayBetweenAttempts ?? new TimeSpan(0, 0, 1));
				}

				_buffer.Complete();

				await Task.WhenAll(_sinks.Select(x => x.Completion).ToArray());
			}
			catch (OperationCanceledException) { }
		}

		public async Task PostEntryAsync(string message, params object[] values)
		{
			await PostEntryAsync(Verbosity.Info, message, values);
		}

		public void PostEntry(string message, params object[] values)
		{
			PostEntry(Verbosity.Info, message, values);
		}

		public async Task PostEntryAsync(Verbosity verbosity, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			await _buffer.SendAsync(new LogEntry(formatted, verbosity));
		}

		public void PostEntry(Verbosity verbosity, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			_buffer.Post(new LogEntry(formatted, verbosity));
		}

		public async Task PostBlankAsync(Verbosity verbosity = Verbosity.Info)
		{
			await _buffer.SendAsync(new LogEntryNoTimestamp(string.Empty, verbosity));
		}

		public void PostBlank(Verbosity verbosity = Verbosity.Info)
		{
			_buffer.Post(new LogEntryNoTimestamp(string.Empty, verbosity));
		}

		public async Task PostEntryNoTimestampAsync(string message, params object[] values)
		{
			await PostEntryNoTimestampAsync(Verbosity.Info, message, values);
		}

		public void PostEntryNoTimestamp(string message, params object[] values)
		{
			PostEntryNoTimestamp(Verbosity.Info, message, values);
		}

		public async Task PostEntryNoTimestampAsync(Verbosity verbosity, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			await _buffer.SendAsync(new LogEntryNoTimestamp(formatted, verbosity));
		}

		public void PostEntryNoTimestamp(Verbosity verbosity, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			_buffer.Post(new LogEntryNoTimestamp(formatted, verbosity));
		}

		public async Task PostExceptionAsync(Exception ex)
		{
			await _buffer.SendAsync(new LogExceptionEntry(ex));
		}

		public void PostException(Exception ex)
		{
			_buffer.Post(new LogExceptionEntry(ex));
		}

		public async Task PostExceptionAsync(Exception ex, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			await _buffer.SendAsync(new LogExceptionEntry(ex, formatted));
		}

		public void PostException(Exception ex, string message, params object[] values)
		{
			var formatted = string.Format(message, values);

			_buffer.Post(new LogExceptionEntry(ex, formatted));
		}

		public void Dispose()
		{
			_buffer.Complete();

			if (_cancellationSource.IsCancellationRequested)
				_cancellationSource.Cancel();

			_cancellationSource.Dispose();
		}

		private readonly CancellationTokenSource _cancellationSource;
		private readonly BufferBlock<ILogEntry> _buffer;
		private readonly BroadcastBlock<ILogEntry> _broadcast;
		private readonly List<ActionBlock<ILogEntry>> _sinks;
	}
}
