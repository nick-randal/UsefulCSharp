// Useful C#
// Copyright (C) 2014 Nicholas Randal
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
using Randal.Core.Dynamic;

namespace Randal.Core.Testing.UnitTest
{
	public abstract class BaseUnitTest<TThens> where TThens : class, new()
	{
		public virtual void Setup()
		{
			Given = Given ?? new DynamicEntity(MissingMemberBehavior.SuccessReturnsNull);
			Given.Clear();

			Then = new TThens();
		}

		protected void When(params Action[] actions)
		{
			foreach (var action in actions)
				action();
		}

		protected dynamic Given;
		protected TThens Then;
	}
}