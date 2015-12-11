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

using Microsoft.SqlServer.Management.Smo;

namespace Randal.Sql.Scripting
{
	public sealed class ScriptableObject
	{
		public ScriptableObject(IScriptingSource scriptingSource, ScriptSchemaObjectBase schemaObject)
		{
			_scriptingSource = scriptingSource;
			_schemaObject = schemaObject;

			_table = _schemaObject as Table;
			_view = _schemaObject as View;
			_sproc = _schemaObject as StoredProcedure;
			_udf = _schemaObject as UserDefinedFunction;
		}

		public IScriptingSource ScriptingSource { get { return _scriptingSource; } }

		public ScriptSchemaObjectBase SchemaObject { get { return _schemaObject; } }

		public bool IsEncrypted
		{
			get
			{
				if (_table != null)
					return false;

				if (_view != null && _view.IsEncrypted)
					return true;

				if (_sproc != null && _sproc.IsEncrypted)
					return true;

				return _udf != null && _udf.IsEncrypted;
			}
		}

		public bool IsTable { get { return _table != null; } }
		public bool IsView { get { return _view != null; } }
		public bool IsSproc { get { return _sproc != null; } }
		public bool IsUdf { get { return _udf != null; } }

		public Table Table { get { return _table; } }
		public View View { get { return _view; } }
		public StoredProcedure Sproc { get { return _sproc; } }
		public UserDefinedFunction Udf { get { return _udf; } }

		private readonly Table _table;
		private readonly View _view;
		private readonly StoredProcedure _sproc;
		private readonly UserDefinedFunction _udf;
		private readonly IScriptingSource _scriptingSource;
		private readonly ScriptSchemaObjectBase _schemaObject;
	}
}