// Useful C#
// Copyright (C) 2014-2022 Nicholas Randal
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
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CSharp.RuntimeBinder;
using Xunit;

namespace GwtUnit.XUnit.Tests
{
	public sealed class DynamicEntityTests
	{
		[Fact, PositiveTest]
		public void ShouldHaveValidObjectWhenCreating()
		{
			dynamic entity = new DynamicEntity();

			Then.Entity = entity;

			Then.Entity.Should().NotBeNull().And.BeAssignableTo<DynamicEntity>();
		}

		[Fact, PositiveTest]
		public void ShouldHaveDynamicPropertyWhenAssignedValue()
		{
			dynamic entity = new DynamicEntity();
			entity.Name = "Jane Doe";

			Then.Entity = entity;
			Then.String = entity.Name;
			Then.Count = entity.Count();

			Then.Entity.Should().NotBeNull().And.BeAssignableTo<DynamicEntity>();
			Then.String.Should().Be("Jane Doe");
			Then.Count.Should().Be(1);
			bool exists = entity.TestForMember("Name");
			exists.Should().BeTrue();
		}

		[Fact, NegativeTest]
		public void ShouldThrowExceptionWhenAccessingNonExistentProperty()
		{
			var f = () =>
			{
				dynamic entity = new DynamicEntity();
				Then.String = entity.Name;
			};

			f.Should().Throw<RuntimeBinderException>().WithMessage("'GwtUnit.XUnit.DynamicEntity' does not contain a definition for 'Name'");
		}


		[Fact, PositiveTest]
		public void ShouldReturnNullWhenAccessingNonExistentPropertyGivenMissingMemberBehaviorReturnsNull()
		{
			dynamic entity = new DynamicEntity(MissingMemberBehavior.ReturnsNull);
			
			Then.String = entity.Name;
			
			Then.String.Should().BeNull();
		}
		
		[Fact, PositiveTest]
		public void ShouldReturnNull_WhenAccessingProperty_GivenReassignedToNull()
		{
			var dynamicEntity = new DynamicEntity(MissingMemberBehavior.ReturnsNull);
			dynamic entity = dynamicEntity; 
			
			entity.Name = "Jane Doe";
			entity.Name = (string?)null!;
			
			(entity.Name as string).Should().BeNull();
			dynamicEntity.TestForMember("Name").Should().BeTrue();
		}

		[Fact, PositiveTest]
		public void ShouldHaveNoMembersWhenClearingAllMembers()
		{
			dynamic entity = new DynamicEntity();
			entity.Name = "Jane Doe";
			entity.Age = 32;

			entity.Clear();
			Then.MemberExists = entity.TestForMember("Name");
			Then.Count = entity.Count();

			Then.MemberExists.Should().BeFalse("all properties were cleared");
			Then.MemberExists = entity.TestForMember("Age");
			Then.MemberExists.Should().BeFalse("all properties were cleared");
			Then.Count.Should().Be(0);
		}

		[Fact, NegativeTest]
		public void ShouldThrowExceptionWhenConvertingToUnexpectedType()
		{
			var f = () =>
			{
				dynamic entity = new DynamicEntity();
				Then.String = entity;
			};

			f.Should().Throw<RuntimeBinderException>().WithMessage("Cannot implicitly convert type 'GwtUnit.XUnit.DynamicEntity' to 'string'");
		}

		[Fact, PositiveTest]
		public void ShouldUseRegisteredConverterWhenConvertingToKnownType()
		{
			dynamic entity = new DynamicEntity(MissingMemberBehavior.ThrowException, new DictionaryConverter());
			entity.Name = "Jane Doe";

			Then.Dictionary = entity;

			Then.Dictionary.Should().NotBeNull().And.HaveCount(1);
		}

		private Thens Then { get; set; } = new();

		private sealed class Thens
		{
			public object Entity;
			public IDictionary<string, object> Dictionary;
			public string String;
			public int Int;
			public int Count;
			public bool MemberExists;
		}
	}
}