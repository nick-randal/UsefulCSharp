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

namespace Randal.Core.Testing.Factory
{
	public class IncrementByMemberValueFactory : IValueFactory
	{
		protected char CurrentChar;
		protected DateTime CurrentDateTime;
		protected DateTime _startingDateTime;

		public IncrementByMemberValueFactory(DateTime startingDateTime)
		{
			_startingDateTime = startingDateTime;
			Reset();
		}

		public void Reset()
		{
			CurrentDateTime = _startingDateTime;
			CurrentChar = 'A';
		}

		public void Increment()
		{
			CurrentDateTime = CurrentDateTime.AddMonths(1);
		}

		public char GetChar(string fieldName)
		{
			var value = CurrentChar;

			CurrentChar++;
			if (CurrentChar > 90)
				CurrentChar = 'A';

			return value;
		}

		public string GetString(string fieldName)
		{
			return fieldName;
		}

		public byte GetByte(string fieldName)
		{
			return (byte)fieldName.GetHashCode();
		}

		public short GetInt16(string fieldName)
		{
			return (short)fieldName.GetHashCode();
		}

		public int GetInt32(string fieldName)
		{
			return fieldName.GetHashCode();
		}

		public long GetInt64(string fieldName)
		{
			return fieldName.GetHashCode();
		}

		public bool GetBool(string fieldName)
		{
			return true;
		}

		public DateTime GetDateTime(string fieldName)
		{
			var value = CurrentDateTime;

			CurrentDateTime = value.AddDays(1);

			return value;
		}

		public decimal GetDecimal(string fieldName)
		{
			return (decimal)fieldName.GetHashCode();
		}

		public float GetFloat(string fieldName)
		{
			return (float)fieldName.GetHashCode();
		}

		public double GetDouble(string fieldName)
		{
			return (double)fieldName.GetHashCode();
		}
	}
}