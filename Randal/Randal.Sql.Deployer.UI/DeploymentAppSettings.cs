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
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Randal.Sql.Deployer.Shared;

namespace Randal.Sql.Deployer.UI
{
	public interface IDeploymentAppSettings
	{
		string ApplicationPath { get; }
		string ProjectFolder { get; }
		string LogFolder { get; }
		string SqlServer { get; }
		bool NoTransaction { get; }
		bool ForceRollback { get; }
		bool CheckFilesOnly { get; }
		bool BypassCheck { get; }
	}

	[DataContract]
	public sealed class DeploymentAppSettings : IDeploymentAppSettings
	{
		[IgnoreDataMember]
		public string ApplicationPath { get; set; }

		[DataMember]
		public string ProjectFolder { get; set; }

		[DataMember]
		public string LogFolder { get; set; }

		[DataMember]
		public string SqlServer { get; set; }

		[DataMember]
		public bool NoTransaction { get; set; }

		[DataMember]
		public bool ForceRollback { get; set; }

		[DataMember]
		public bool CheckFilesOnly { get; set; }

		[DataMember]
		public bool BypassCheck { get; set; }

		public override string ToString()
		{
			var text = new StringBuilder();

			text.AppendFormat(@"-p ""{0}"" -l ""{1}"" -s ""{2}"" -e ""{3}""", 
				ProjectFolder, LogFolder, SqlServer, SharedConst.LogExchangeNetPipe);

			if (NoTransaction)
				text.Append(" -n");

			if (ForceRollback)
				text.Append(" -r");

			if (CheckFilesOnly)
				text.Append(" -c");

			if (BypassCheck)
				text.Append(" -b");

			return text.ToString();
		}

		public async Task Save(string path = null)
		{
			path = path ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LastSettingsFileName);
			var file = new FileInfo(path);

			using (var writer = new StreamWriter(file.Open(FileMode.Create, FileAccess.Write, FileShare.Read)))
			{
				await writer.WriteAsync(
					JsonConvert.SerializeObject(this, Formatting.Indented)
				);
			}
		}

		public static async Task<DeploymentAppSettings> Load(string path = null)
		{
			path = path ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LastSettingsFileName);
			var file = new FileInfo(path);

			if (file.Exists == false)
				return null;

			using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
			{
				var text = await reader.ReadToEndAsync();
				return JsonConvert.DeserializeObject<DeploymentAppSettings>(text);
			}
		}

		private const string LastSettingsFileName = "last.cfg";
	}
}