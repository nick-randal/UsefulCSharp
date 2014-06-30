using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randal.Sql.Scripting
{
	public interface IScriptFormatter
	{
		string Format(StoredProcedure sproc);
	}

	public sealed class ScriptFormatter
	{
	}
}
