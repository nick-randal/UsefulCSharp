﻿// Useful C#
// Copyright (C) 2015-2016 Nicholas Randal
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

namespace Randal.Core.T4
{
	public static class Constants
	{
		public static string AutoGeneratedWarning()
		{
			return string.Format(Text.AutoGeneratedWarningText, DateTime.Now.ToString("G"));
		}

		internal static class Text
		{
			
			public const string 
			AutoGeneratedWarningText =
			@"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Last generated on: {0}
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------"
		;
		}
	}
}