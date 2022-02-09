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

namespace GwtUnit.XUnit
{
	public interface ITestObjectBuilder<out TBuild>
	{
		TBuild Build();
	}

	public interface IChildTestObjectBuilder<out TBuild, out TParentBuilder> : ITestObjectBuilder<TBuild>
	{
		TParentBuilder ParentBuilder { get; }
	}
}