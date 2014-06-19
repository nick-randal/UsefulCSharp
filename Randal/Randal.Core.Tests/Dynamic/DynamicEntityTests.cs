/*
Useful C#
Copyright (C) 2014  Nicholas Randal

Useful C# is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Dynamic;

namespace Randal.Tests.Core.Dynamic
{
	[TestClass]
	public sealed class DynamicEntityTests
	{
		[TestInitialize]
		public void Setup()
		{
			Then = new Thens();
		}

		[TestMethod]
		public void ShouldHaveValidObjectWhenCreating()
		{
			dynamic entity = new DynamicEntity();
			
			Then.Entity = entity;
			Then.Entity.Should().NotBeNull().And.BeAssignableTo<DynamicEntity>();
		}

		[TestMethod]
		public void ShouldHaveDynamicPropertyWhenAssignedValue()
		{
			dynamic entity = new DynamicEntity();
			entity.Name = "Jane Doe";
			Then.String = entity.Name;

			Then.Entity = entity;
			Then.Entity.Should().NotBeNull().And.BeAssignableTo<DynamicEntity>();
			Then.String.Should().Be("Jane Doe");
			bool exists = entity.TestForMember("Name");
			exists.Should().BeTrue();
		}

		[TestMethod, ExpectedException(typeof(RuntimeBinderException))]
		public void ShouldThrowExceptionWhenAccessingNonExistentProperty()
		{
			dynamic entity = new DynamicEntity();
			Then.String = entity.Name;
		}


		[TestMethod]
		public void ShouldReturnNullWhenAccessingNonExistentPropertyGivenMissingMemberBehaviorReturnsNull()
		{
			dynamic entity = new DynamicEntity(MissingMemberBehavior.SuccessReturnsNull);
			Then.String = entity.Name;
			Then.String.Should().BeNull();
		}

		[TestMethod]
		public void ShouldHaveNoMembersWhenClearingAllMembers()
		{
			dynamic entity = new DynamicEntity();
			entity.Name = "Jane Doe";
			entity.Age = 32;

			entity.Clear();

			bool exists = entity.TestForMember("Name");
			exists.Should().BeFalse("all properties were cleared");
			exists = entity.TestForMember("Age");
			exists.Should().BeFalse("all properties were cleared");
		}

		[TestMethod, ExpectedException(typeof(RuntimeBinderException))]
		public void ShouldThrowExceptionWhenConvertingToUnexpectedType()
		{
			dynamic entity = new DynamicEntity();

			Then.String = entity;
		}

		[TestMethod]
		public void ShouldUseRegisteredConverterWhenConvertingToKnownType()
		{
			dynamic entity = new DynamicEntity(MissingMemberBehavior.ThrowException, new DictionaryConverter());
			entity.Name = "Jane Doe";

			Then.Dictionary = entity;

			Then.Dictionary.Should().NotBeNull().And.HaveCount(1);
		}

		private Thens Then { get; set; }

		public class Thens
		{
			public object Entity;
			public IDictionary<string, object> Dictionary;
			public string String;
			public int Int;
		}
	}
}
