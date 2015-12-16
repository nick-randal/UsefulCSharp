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

namespace Randal.Logging
{
	public sealed class LogFile : ILogFile
	{
		public LogFile(string filePath, long sizeInBytes)
		{
			_file = new FileInfo(filePath);
			State = LogFileState.Unknown;
			_sizeInBytes = sizeInBytes;
		}

		public string FilePath
		{
			get { return _file.FullName; }
		}

		public void Open()
		{
			CheckIfDisposed();

			if (_state != LogFileState.Unknown)
				throw new InvalidOperationException("Cannot open an already opened file");

			CreateStreamWriter();

			var firstNullPosition = FindFirstNull();
			if (firstNullPosition < 0 || firstNullPosition >= _sizeInBytes)
			{
				_writer.WriteLine("Cutover");
				State = LogFileState.OpenExhausted;
			}
			else if (firstNullPosition == 0)
			{
				_writer.BaseStream.Position = 0;
				State = LogFileState.OpenAvailable;
			}
			else
			{
				_writer.BaseStream.Position = firstNullPosition;
				_writer.WriteLine();
				State = LogFileState.OpenAvailable;
			}
		}

		private void CreateStreamWriter()
		{
			_writer =
				new StreamWriter(new FileStream(_file.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
				{
					AutoFlush = true
				};

			if (_writer.BaseStream.Length < _sizeInBytes)
				_writer.BaseStream.SetLength(_sizeInBytes);
		}

		private void RefreshState()
		{
			if (_writer == null)
				return;

			if (_state == LogFileState.OpenAvailable && _writer.BaseStream.Position >= _sizeInBytes)
				_state = LogFileState.OpenExhausted;
		}

		private long FindFirstNull()
		{
			var buffer = new byte[BufferLength];
			int bytesRead;
			var stream = _writer.BaseStream;

			do
			{
				bytesRead = stream.Read(buffer, 0, BufferLength);

				for (var n = 0; n < BufferLength; n++)
				{
					if (buffer[n] == 0)
						return stream.Position - bytesRead + n;
				}
			} while (bytesRead > 0);

			return -1;
		}

		public void Close()
		{
			CheckIfDisposed();

			if (_writer == null)
				return;

			try
			{
				_writer.Flush();
				_writer.Close();
				_writer.Dispose();
			}
			finally
			{
				_state = LogFileState.Unknown;
				_writer = null;
			}
		}

		public LogFileState State
		{
			get
			{
				RefreshState();
				return _state;
			}
			private set { _state = value; }
		}

		public long SizeInBytes
		{
			get { return _sizeInBytes; }
		}

		public StreamWriter GetStreamWriter()
		{
			RefreshState();

			if (_state == LogFileState.OpenAvailable || _state == LogFileState.OpenExhausted)
				return _writer;

			return null;
		}

		private void CheckIfDisposed()
		{
			if (_state == LogFileState.Disposed)
				throw new ObjectDisposedException("LogFile");
		}

		public void Dispose()
		{
			Close();
			_state = LogFileState.Disposed;
		}

		private LogFileState _state;
		private StreamWriter _writer;
		private readonly long _sizeInBytes;
		private readonly FileInfo _file;

		public const int BufferLength = 4096;
	}
}