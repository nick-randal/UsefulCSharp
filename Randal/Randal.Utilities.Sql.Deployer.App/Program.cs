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

using CommandLine;

namespace Randal.Sql.Deployer.App
{
	internal sealed class SqlDeployerProgram
	{
		private static int Main(string[] args)
		{
			var options = new AppOptions();
			if (Parser.Default.ParseArguments(args, options) == false)
				return 2;

			var settings = new RunnerSettings(options.ProjectFolder, options.LogFolder, options.Server, options.Rollback,
				options.NoTransaction);
			var runner = new Runner(settings);
			runner.Go();

			return -1;
		}
	}
}