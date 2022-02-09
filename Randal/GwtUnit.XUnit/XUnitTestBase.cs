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

using System.Linq;

namespace GwtUnit.XUnit
{
	public abstract class XUnitTestBase<TThens> : XUnitTestBase<TThens, dynamic>
		where TThens : class, new()
	{
		protected XUnitTestBase()
		{
			Given = new DynamicEntity(MissingMemberBehavior.ReturnsNull);
			Then = new TThens();
		}

		public new readonly dynamic Given;

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
		/// Get the value for a Given.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="value"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>True - if the Given is defined, False - not defined.</returns>
		protected bool TryGiven<T>(string member, out T? value)
		{
			if (Given.TestForMember(member))
			{
				value = Given[member];
				return true;
			}

			value = default;
			return false;
		} 

		/// <summary>
		/// Return the Given value if defined or default value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="member"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		protected T? GivenOrDefault<T>(string member, T? defaultValue = default)
		{
			return Given.TestForMember(member) ? (T)Given[member] : defaultValue;
		}
	}
}