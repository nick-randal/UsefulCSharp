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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Dynamic;

namespace Randal.Core.Testing.UnitTest
{
	[TestClass]
	public abstract class BaseUnitTest<TThens> where TThens : class, new()
	{
		[TestInitialize]
		public void Setup()
		{
			Given = Given ?? new DynamicEntity(MissingMemberBehavior.ReturnsNull);
			Then = new TThens();

			OnSetup();
		}

		[TestCleanup]
		public void Teardown()
		{
			OnTeardown();

			var disposeMe = Then as IDisposable;
			if (disposeMe != null)
				disposeMe.Dispose();

			Given.Clear();
			Then = null;
		}

		protected virtual void OnSetup() { }

		protected virtual void OnTeardown() { }

		protected void When(params Action[] actions)
		{
			foreach (var action in actions)
				action();
		}

		protected bool GivensDefined(params string[] members)
		{
			if (members.Length == 0)
				return true;

			return members.All(member => Given.TestForMember(member));
		}

		protected dynamic Given;
		protected TThens Then;
	}
}