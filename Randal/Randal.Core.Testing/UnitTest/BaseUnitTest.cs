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
