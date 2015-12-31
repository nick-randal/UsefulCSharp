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

using System;
using Randal.Sql.Deployer.Configuration;
using Randal.Sql.Deployer.Scripts;

namespace Randal.Sql.Deployer.Process
{
	public abstract class ScriptDeployerBase : IScriptDeployer
	{
		protected readonly IProject Project;

		protected ScriptDeployerBase(IScriptDeployerConfig config, IProject project)
		{
			DeployerConfig = config;
			Project = project;
		}

		public abstract bool CanProceed();

		public abstract Core.Enums.Returned DeployScripts();

		public IScriptDeployerConfig DeployerConfig { get; protected set; }

		public virtual void Dispose()
		{
			
		}

		protected string PhaseDeploymentComment
		{
			get
			{
				return string.Format(
					"-- deployed '{1}' v{2} on {3} {0}{0}",
					Environment.NewLine,
					Project.Configuration.Project,
					Project.Configuration.Version,
					DateTime.Now);
			}
		}
	}
}