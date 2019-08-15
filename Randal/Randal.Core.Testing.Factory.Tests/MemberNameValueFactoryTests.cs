// Useful C#
// Copyright (C) 2014-2019 Nicholas Randal
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
using FluentAssertions;
using GwtUnit.XUnit;
using Xunit;

namespace Randal.Core.Testing.Factory.Tests
{
	/// <summary>
	/// Created by nrandal on 9/10/2015 10:24:57 AM
	/// </summary>
	public sealed class MemberNameValueFactoryTests : XUnitTestBase<MemberNameValueFactoryTests.Thens>
	{
		[Fact, PositiveTest]
		public void ShouldHaveFirstValue_WhenGettingString()
		{
			When(GettingString);

			Then.StringValue.Should().Be("FirstName");
		}

		[Fact, PositiveTest]
		public void ShouldHaveFirstValue_WhenGettingInt()
		{
			When(GettingInt);

			Then.IntValue.Should().Be(717046770);
		}

		[Fact, PositiveTest]
		public void ShouldHaveFirstValue_WhenGettingShort()
		{
			When(GettingShort);

			Then.ShortValue.Should().Be(-14302);
		}

		[Fact, PositiveTest]
		public void ShouldHaveFirstValue_WhenGettingDecimal()
		{
			When(GettingDecimal);

			Then.DecimalValue.Should().Be(2478816839);
		}

		[Fact, PositiveTest]
		public void ShouldHaveFirstValue_WhenGettingDateTime()
		{
			When(GettingDateTime);

			Then.DateTimeValue.Should().Be(new DateTime(2001, 11, 11, 11, 0, 0));
		}

		[Fact, PositiveTest]
		public void ShouldHaveFirstValue_WhenGettingChar()
		{
			When(GettingChar);

			Then.CharValue.Should().Be('l');
		}

		protected override void Creating()
		{
			Then.Target = new MemberNameValueFactory();
		}

		private void GettingString()
		{
			Then.StringValue = Then.Target.GetString("FirstName");
		}

		private void GettingInt()
		{
			Then.IntValue = Then.Target.GetInt32("Id");
		}

		private void GettingShort()
		{
			Then.ShortValue = Then.Target.GetInt16("Elevation");
		}

		private void GettingDecimal()
		{
			Then.DecimalValue = Then.Target.GetDecimal("Salary");
		}

		private void GettingDateTime()
		{
			Then.DateTimeValue = Then.Target.GetDateTime("CreatedOn");
		}

		private void GettingChar()
		{
			Then.CharValue = Then.Target.GetChar("Grade");
		}

		public sealed class Thens
		{
			public MemberNameValueFactory Target;
			public string StringValue;
			public int IntValue;
			public short ShortValue;
			public decimal DecimalValue;
			public DateTime DateTimeValue;
			public char CharValue;
		}
	}
}