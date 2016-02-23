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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Dynamic;

namespace Randal.Core.Testing.UnitTest
{
	[TestClass]
	public abstract class UnitTestBase<TThens> : UnitTestBase<TThens, dynamic>
		where TThens : class, new()
	{
		[TestInitialize]
		public new void Setup()
		{
			if(Given == null)
				Given = new DynamicEntity(MissingMemberBehavior.ReturnsNull);
			else
				Given.Clear();
			
			Then = new TThens();

			OnSetup();
		}

		public new dynamic Given;

		/// <summary>
		/// Determine if all provided members have been defined as Given values.
		/// </summary>
		/// <param name="members">A list of property names</param>
		/// <returns>True if all properties specified are defined, otherwise False.</returns>
		protected bool GivensDefined(params string[] members)
		{
			return members.Length == 0 || members.All(member => Given.TestForMember(member));
		}
	}
}