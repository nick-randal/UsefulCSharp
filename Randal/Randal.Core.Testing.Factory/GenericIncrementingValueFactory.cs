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

namespace Randal.Core.Testing.Factory
{
	public class GenericIncrementingValueFactory : IValueFactory
	{
		protected byte CurrentByte;
		protected char CurrentChar;
		protected DateTime CurrentDateTime;
		protected double CurrentDouble;
		protected bool CurrentFlag;
		protected float CurrentFloat;
		protected int CurrentInt;
		protected long CurrentLong;
		protected decimal CurrentDecimal;
		protected short CurrentShort;
		protected int CurrentString;

		public GenericIncrementingValueFactory ()
		{
			Reset();
		}

		public void Reset()
		{
			CurrentByte = 1;
			CurrentChar = 'A';
			CurrentDateTime = DateTime.Today;
			CurrentDecimal = 1m;
			CurrentDouble = 1d;
			CurrentFlag = false;
			CurrentFloat = 1f;
			CurrentInt = 1;
			CurrentLong = 1;
			CurrentShort = 1;
			CurrentString = 1;
		}

		public char GetChar(string fieldName)
		{
			var capture = CurrentChar++;
			
			if(CurrentChar > 90)
				CurrentChar = 'A';

			return capture;
		}

		public string GetString(string fieldName)
		{
			return "value" + CurrentString++;
		}

		public byte GetByte(string fieldName)
		{
			return CurrentByte++;
		}

		public short GetInt16(string fieldName)
		{
			return CurrentShort++;
		}

		public int GetInt32(string fieldName)
		{
			return CurrentInt++;
		}

		public long GetInt64(string fieldName)
		{
			return CurrentLong++;
		}

		public bool GetBool(string fieldName)
		{
			var capture = CurrentFlag;
			CurrentFlag = !CurrentFlag;
			return capture;
		}

		public DateTime GetDateTime(string fieldName)
		{
			var capture = CurrentDateTime;
			CurrentDateTime = CurrentDateTime.AddHours(1);
			return capture;
		}

		public decimal GetDecimal(string fieldName)
		{
			return CurrentDecimal++;
		}

		public float GetFloat(string fieldName)
		{
			return CurrentFloat++;
		}

		public double GetDouble(string fieldName)
		{
			return CurrentDouble++;
		}
	}
}