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
	public interface IValueFactory
	{
		void Reset();

		void Increment();

		char GetChar(string fieldName);
		string GetString(string fieldName);

		byte GetByte(string fieldName);
		short GetInt16(string fieldName);
		int GetInt32(string fieldName);
		long GetInt64(string fieldName);

		bool GetBool(string fieldName);

		DateTime GetDateTime(string fieldName);

		decimal GetDecimal(string fieldName);
		float GetFloat(string fieldName);
		double GetDouble(string fieldName);
	}
}