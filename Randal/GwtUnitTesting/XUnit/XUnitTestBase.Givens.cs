﻿// Useful C#
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
using System.Linq;
using System.Threading.Tasks;

namespace GwtUnit.XUnit
{
	public abstract class XUnitTestBase<TThens, TGivens> : IDisposable
		where TThens : class, new()
		where TGivens : class, new()
	{
		protected XUnitTestBase()
		{
			Given = new TGivens();
			Then = new TThens();
		}

		public virtual void Dispose()
		{
			var disposeMe = Given as IDisposable;
			disposeMe?.Dispose();

			disposeMe = Then as IDisposable;
			disposeMe?.Dispose();

			Then = null;
		}

		/// <summary>
		/// Will execute each action provided, in order.  If Creating was not provided as an action, Creating will be called automatically as the first action.
		/// However, if a Creating is provided then it will not be called automatically and it is assumed that the caller wants full control of actions and the order.
		/// </summary>
		/// <param name="actions">A list of actions to be performed for the current test</param>
		protected void When(params Action[] actions)
		{
			if (actions.Any(a => a == Creating) == false && actions.Any(a => a == NotCreating) == false)
				Creating();

			foreach (var action in actions)
				action();
		}

		/// <summary>
		/// Will execute each action provided, in order, except for the last one.  That action will be set as ThenLastAction and will not be executed.
		/// This is done so that the action can be executed in conjunction with an assertion mechanism other than MSTest's ExpectedException attribute.
		/// </summary>
		/// <param name="actions"></param>
		protected void WhenLastActionDeferred(params Action[] actions)
		{
			var listOfActions = actions.ToList();

			if (actions.Any(a => a == Creating) == false && actions.Any(a => a == NotCreating) == false)
				listOfActions.Insert(0, Creating);

			for (var n = 0; n < listOfActions.Count - 1; n++)
				listOfActions[n]();

			ThenLastAction = listOfActions.Last();
		}

		protected Action Repeat(Action action, int repeatX)
		{
			if (repeatX < 1)
				repeatX = 1;

			return () =>
			{
				for (var n = 0; n < repeatX; n++)
					action();
			};
		}

		protected Action Await(Func<Task> asyncFunc)
		{
			return () =>
			{
				using (ThenLastTask = Task.Run(async () => await asyncFunc()))
				{
					ThenLastTask.Wait();
				}
			};
		}

		protected abstract void Creating();

		protected Action NotCreating = () => { };

		protected TGivens Given;

		protected TThens Then;

		protected Action ThenLastAction;

		protected Task ThenLastTask;
	}
}