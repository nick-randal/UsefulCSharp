using System;

namespace Randal.Core.Testing.Factory
{
	[Flags]
	public enum PrepareOptions
	{
		Default = 0,
		IncludePrivateFields = 1,
		IncludePrivateProperties = 2
	}
}