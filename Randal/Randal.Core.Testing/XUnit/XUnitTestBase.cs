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
using Randal.Core.Dynamic;

namespace GwtUnit.XUnit
{
	public abstract class XUnitTestBase<TThens> : XUnitTestBase<TThens, dynamic>
		where TThens : class, new()
	{
		protected XUnitTestBase()
		{
			if(Given == null)
				Given = new DynamicEntity(MissingMemberBehavior.ReturnsNull);
			else
				Given.Clear();
			
			Then = new TThens();
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

		/// <summary>
		/// Return the Given value if defined or default value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="member"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		protected T GivenOrDefault<T>(string member, T defaultValue)
		{
			return Given.TestForMember(member) ? Given[member] : defaultValue;
		}
	}
}