/*
Useful C#
Copyright (C) 2014  Nicholas Randal

Useful C# is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/

using System;
using Randal.Core.Enums;
using Randal.Core.IO.Logging;
using Randal.Utilities.Sql.Deployer.IO;
using Randal.Utilities.Sql.Deployer.Process;
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Utilities.Sql.Deployer.App
{
	public sealed class Runner : IDisposable
	{
		public Runner(RunnerSettings settings, ILogger logger = null)
		{
			if(settings == null)
				throw new ArgumentNullException("settings");

			_settings = settings;
			
			if(logger == null)
				logger = new AsyncFileLogger(settings.FileLoggerSettings);
			var decorator = logger as ILoggerStringFormatDecorator;
			_logger = decorator ?? new LoggerStringFormatDecorator(logger);
		}

		public void Go()
		{
			var commit = false;

			using (var connectionManager = new SqlConnectionManager())
			{
				try
				{
					LogOptions();

					_logger.AddEntry("opening connection to server", _settings.Server);
					connectionManager.OpenConnection(_settings.Server, TextResources.Sql.Database.Master);

					if (_settings.UseTransaction)
					{
						_logger.AddEntry("beginning transaction");
						connectionManager.BeginTransaction();
					}

					var project = LoadProject(CreateParser());

					DeployScripts(project, connectionManager);

					commit = _settings.ShouldRollback == false;
				}
				catch (RunnerException ex)
				{
					_logger.Add(new ExceptionEntry(ex));
				}
				finally
				{
					ResolveTransaction(commit, connectionManager);
				}
			}
		}

		private void ResolveTransaction(bool commit, ISqlConnectionManager connectionManager)
		{
			_logger.AddEntryNoTimestamp(string.Empty);

			if (commit)
			{
				_logger.AddEntry(Verbosity.Vital, "~~~~~~~~~~ COMMITTING ~~~~~~~~~~{0}", Environment.NewLine);
				connectionManager.CommitTransaction();
				return;
			}

			_logger.AddEntry(Verbosity.Vital, "~~~~~~~~~~ ROLLING BACK ~~~~~~~~~~{0}", Environment.NewLine);
			connectionManager.RollbackTransaction();
		}

		private void LogOptions()
		{
			_logger.AddEntry("runner options");
			_logger.AddEntryNoTimestamp("server          :  {0}", _settings.Server);
			_logger.AddEntryNoTimestamp("project folder  :  {0}", _settings.ScriptProjectFolder);
			_logger.AddEntryNoTimestamp("log folder      :  {0}", _settings.FileLoggerSettings.BasePath);
			_logger.AddEntryNoTimestamp("use transaction :  {0}", _settings.UseTransaction);
			_logger.AddEntryNoTimestamp("rollback trans  :  {0}", _settings.ShouldRollback);
		}

		private static IScriptParserConsumer CreateParser()
		{
			var parser = new ScriptParser();

			parser.AddRule(ScriptParser.StandardKeys.Catalog, txt => new CatalogBlock(txt));
			parser.AddRule(ScriptParser.StandardKeys.Options, txt => new OptionsBlock(txt));
			parser.AddRule(ScriptParser.StandardKeys.Need, txt => new NeedBlock(txt));
			parser.AddRule(ScriptParser.StandardKeys.Ignore, txt => new IgnoreScriptBlock(txt));
			parser.AddRule(ScriptParser.StandardKeys.Pre, txt => new SqlCommandBlock("pre", txt, SqlScriptPhase.Pre));
			parser.AddRule(ScriptParser.StandardKeys.Main, txt => new SqlCommandBlock("main", txt, SqlScriptPhase.Main));
			parser.AddRule(ScriptParser.StandardKeys.Post, txt => new SqlCommandBlock("post", txt, SqlScriptPhase.Post));

			parser.SetFallbackRule((kw, txt) => new UnexpectedBlock(kw, txt));

			return parser;
		}

		private Project LoadProject(IScriptParserConsumer parser)
		{
			var loader = new ProjectLoader(_settings.ScriptProjectFolder, parser, _logger);
			if (loader.Load() == Returned.Failure)
				throw new RunnerException("Failed to load project");

			return new Project(loader.Configuration, loader.AllScripts);
		}

		private void DeployScripts(IProject project, ISqlConnectionManager connectionManager)
		{
			var deployer = new ScriptDeployer(project, connectionManager, _logger);

			if (deployer.CanUpgrade() == false)
				throw new RunnerException("Cannot upgrade project");

			if (deployer.DeployScripts() == Returned.Failure)
				throw new RunnerException("Deploy scripts failed.");
		}

		public void Dispose()
		{
			if(_logger != null)
				_logger.Dispose();
		}

		private readonly ILoggerStringFormatDecorator _logger;
		private readonly RunnerSettings _settings;
	}
}