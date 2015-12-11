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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Randal.Sql.Deployer.Shared;

namespace Randal.Sql.Deployer.UI
{
	public sealed class ScriptDeploymentProcess
	{
		public async Task Run(IProgress<string> progressOutput, IProgress<string> progressError, IDeploymentAppSettings settings)
		{
			var processInfo = CreateProcessStartInfo(settings);

			await Task.Factory.StartNew(() =>
			{
				try
				{
					using (var process = Process.Start(processInfo))
					{
						if (process == null)
							return;

						using (var taskReadOutput = Task.Factory.StartNew(state => ReadFromStreamAsync(state, progressOutput), process.StandardOutput))
						using (var taskReadError = Task.Factory.StartNew(state => ReadFromStreamAsync(state, progressError), process.StandardError))
						{
							process.WaitForExit();
							Task.WaitAll(new Task[] { taskReadOutput, taskReadError }, 10000);

							progressOutput.Report(Environment.NewLine + "Deployer exited : " + DeployerResolution(process.ExitCode));

							if(string.IsNullOrWhiteSpace(LogExchange.LogFilePath))
								progressOutput.Report("Check log for information.");
							else
								progressOutput.Report("Check log file at link:[" + LogExchange.LogFilePath + ']');
						}
					}
				}
				catch (Exception ex)
				{
					progressError.Report("Error: " + ex.Message);
				}
			});
		}

		private static string DeployerResolution(int exitCode)
		{
			string appendMessage;

			if (Enum.IsDefined(typeof (RunnerResolution), exitCode) == false)
				appendMessage = "Unexpected process exit code";
			else
			{
				var resolution = (RunnerResolution) exitCode;

				var type = resolution.GetType();
				var da = type.GetField(type.GetEnumName(resolution)).GetCustomAttribute<DisplayAttribute>();
				appendMessage = da.Name + " - " + da.Description;
			}

			return "(" + exitCode + ") " + appendMessage;
		}

		private static ProcessStartInfo CreateProcessStartInfo(IDeploymentAppSettings settings)
		{
			var workingDir = Path.GetDirectoryName(settings.ApplicationPath) ?? AppDomain.CurrentDomain.BaseDirectory;

			return new ProcessStartInfo
			{
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				FileName = settings.ApplicationPath,
				WorkingDirectory = workingDir,
				Arguments = settings.ToString(),
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				ErrorDialog = false
			};
		}

		private static async Task ReadFromStreamAsync(object state, IProgress<string> progress)
		{
			try
			{
				var reader = state as StreamReader;
				if (reader == null)
					return;

				do
				{
					var text = await reader.ReadLineAsync();
					if (text == null)
						return;

					progress.Report(text);
				} while (true);
			}
			catch (Exception ex)
			{
				progress.Report("Error: " + ex.Message);
			}
		}
	}
}