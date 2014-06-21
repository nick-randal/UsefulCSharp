using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randal.Core.Testing.UnitTest
{
	public interface ITestObjectBuilder<TBuild>
	{
		TBuild Build();
	}
}
