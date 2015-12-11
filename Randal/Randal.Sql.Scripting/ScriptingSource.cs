using System;
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

using System.Collections.Generic;
using Microsoft.SqlServer.Management.Smo;

namespace Randal.Sql.Scripting
{
	public interface IScriptingSource
	{
		string SubFolder { get; }
		IEnumerable<ScriptSchemaObjectBase> GetScriptableObjects(IServer server, Database database);
	}

	public sealed class ScriptingSource : IScriptingSource
	{
		public ScriptingSource(string subFolder, Func<IServer, Database, IEnumerable<ScriptSchemaObjectBase>> getObjectsFunc)
		{
			_getObjectsFunc = getObjectsFunc;
			SubFolder = subFolder;
		}

		public string SubFolder { get; private set; }

		public IEnumerable<ScriptSchemaObjectBase> GetScriptableObjects(IServer server, Database database)
		{
			return _getObjectsFunc(server, database);
		}

		private readonly Func<IServer, Database, IEnumerable<ScriptSchemaObjectBase>> _getObjectsFunc;
	}
}