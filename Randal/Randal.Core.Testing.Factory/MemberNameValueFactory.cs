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
	public class MemberNameValueFactory : IValueFactory
	{
		protected char CurrentChar;
		protected DateTime CurrentDateTime;
		protected DateTime StartingDateTime;

		public MemberNameValueFactory(DateTime? startingDateTime = null)
		{
			StartingDateTime = startingDateTime ?? new DateTime(2000, 1, 1);
			Reset();
		}

		public void Reset() { }

		public void Increment() { }

		public char GetChar(string fieldName)
		{
			return (char)Math.Abs(48 + (fieldName.ToCrc32() % 74));
		}

		public string GetString(string fieldName)
		{
			return fieldName;
		}

		public byte GetByte(string fieldName)
		{
			return (byte)fieldName.ToCrc32();
		}

		public short GetInt16(string fieldName)
		{
			return (short)fieldName.ToCrc32();
		}

		public int GetInt32(string fieldName)
		{
			return (int)fieldName.ToCrc32();
		}

		public long GetInt64(string fieldName)
		{
			return fieldName.ToCrc32();
		}

		public bool GetBool(string fieldName)
		{
			return (fieldName.ToCrc32() % 2) == 0;
		}

		public DateTime GetDateTime(string fieldName)
		{
			var addHours = fieldName.ToCrc32() % 142000;

			return StartingDateTime.AddHours(addHours);
		}

		public decimal GetDecimal(string fieldName)
		{
			return fieldName.ToCrc32();
		}

		public float GetFloat(string fieldName)
		{
			return fieldName.ToCrc32();
		}

		public double GetDouble(string fieldName)
		{
			return fieldName.ToCrc32();
		}
	}
}