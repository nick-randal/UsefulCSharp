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

namespace Randal.Core.Factories
{
	public sealed class RandomFactory
	{
		public RandomFactory()
		{
			_random = new Random((int)DateTime.UtcNow.Ticks);
			_nameBuffer = new char[25];
		}

		public Random InternalRandom
		{
			get { return _random; }
		}

		public int NameLength
		{
			get { return _nameBuffer.Length; }
			set
			{
				if (value < 1)
					value = 1;

				_nameBuffer = new char[value];
			}
		}

		public string Name
		{
			get
			{
				var first = true;
				var len = _random.Next(3, _nameBuffer.Length);

				for (var n = 0; n < len; n++)
				{
					if (first)
					{
						_nameBuffer[n] = (char)_random.Next(65, 90);
						first = false;
					}
					else
						_nameBuffer[n] = (char)_random.Next(97, 122);
				}

				return new String(_nameBuffer, 0, len);
			}
		}

		public int Int(int min = int.MinValue, int max = int.MaxValue)
		{
			return _random.Next(min, max);
		}

		public uint UInt(int min = 0, int max = int.MaxValue)
		{
			return (uint)_random.Next(min, max);
		}

		public byte Byte(int min = 0, int max = 256)
		{
			return (byte)_random.Next(min, max);
		}

		private char[] _nameBuffer;
		private readonly Random _random;
	}
}