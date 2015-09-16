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

namespace Randal.Core.Testing.Factory.Tests
{
	public sealed class TestModel
	{
		public int PrimaryId { get; set; }

		public int Age;

		public string FirstName { get; set; }

		public string LastName;

		public long SecondaryId { get; set; }

		public long OtherId;

		public bool IsSomething { get; set; }

		public bool WasSomething;

		public char MiddleInitial { get; set; }

		public char Status;

		public byte Flags1 { get; set; }

		public byte Flags2;

		public DateTime CreatedOn { get; set; }

		public DateTime ChangedOn;

		public decimal Salary { get; set; }

		public decimal Bonus;

		public short SquareFeet { get; set; }

		public short Acreage;

		public float Longitude { get; set; }

		public float Latitude { get; set; }

		public double BigPrecision { get; set; }

		public int? TempId1 { get; set; }

		public int? TempId2;

		public object ChildObject;

		public OtherModel ChildModel1 { get; set; }

		public OtherModel ChildModel2;
	}
}