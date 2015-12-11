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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.SqlServer.Management.Smo;
using Randal.Sql.Deployer.UI.Support;

namespace Randal.Sql.Deployer.UI
{
	public sealed class ViewModel : INotifyPropertyChanged
	{
		private readonly IWindowWrapper _windowWrapper;

		public ViewModel(IDeploymentAppSettings appSettings, IWindowWrapper windowWrapper)
		{
			_windowWrapper = windowWrapper;
			IsBusy = false;

			ProjectFolder = appSettings.ProjectFolder;
			LogFolder = appSettings.LogFolder;
			SqlServer = appSettings.SqlServer;
			NoTransaction = appSettings.NoTransaction;
			ForceRollback = appSettings.ForceRollback;
			CheckFilesOnly = appSettings.CheckFilesOnly;
			BypassCheck = appSettings.BypassCheck;

			_serversList = new List<ComboBoxItem>
			{
				new ComboBoxItem { Content = "localhost" },
				new ComboBoxItem { Content = "searching for servers...", IsEnabled = false}
			};

			_projectFolderCommand = new DelegateCommand<string>(s =>
			{
				string selectedPath;
				if (_windowWrapper.ShowFolderDialog("Select Project Folder", s, out selectedPath))
					ProjectFolder = selectedPath;
			}, s => IsAvailable);

			_logFolderCommand = new DelegateCommand<string>(s =>
			{
				string selectedPath;
				if (_windowWrapper.ShowFolderDialog("Select Log Folder", s, out selectedPath))
					LogFolder = selectedPath;

			}, s => IsAvailable);
		}

		public DelegateCommand<string> ProjectFolderSelectCommand { get { return _projectFolderCommand; } }

		public DelegateCommand<string> LogFolderSelectCommand { get { return _logFolderCommand; } }

		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				if (_isBusy == value)
					return;

				_isBusy = value;
				NotifyPropertyChanged("IsBusy", "IsAvailable");
			}
		}

		public bool IsAvailable
		{
			get { return !_isBusy; }
		}

		public List<ComboBoxItem> ServersList
		{
			get { return _serversList; }
			set
			{
				if (value == null)
					return;

				_serversList = value;
				NotifyPropertyChanged("ServersList");
			}
		}

		public string ProjectFolder
		{
			get { return _projectFolder; }
			set
			{
				if (_projectFolder == value)
					return;

				_projectFolder = value.Trim();
				NotifyPropertyChanged("ProjectFolder");
			}
		}

		public string LogFolder
		{
			get { return _logFolder; }
			set
			{
				if (_logFolder == value)
					return;

				_logFolder = value.Trim();
				NotifyPropertyChanged("LogFolder");
			}
		}

		public string SqlServer
		{
			get { return _sqlServer; }
			set
			{
				if (_sqlServer == value)
					return;

				_sqlServer = value.Trim();
				NotifyPropertyChanged("SqlServer");
			}
		}

		public bool NoTransaction
		{
			get { return _noTransaction; }
			set
			{
				if (_noTransaction == value)
					return;

				_noTransaction = value;
				NotifyPropertyChanged("NoTransaction");

				if (_noTransaction)
					ForceRollback = false;
			}
		}

		public bool ForceRollback
		{
			get { return _forceRollback; }
			set
			{
				if (_forceRollback == value)
					return;

				_forceRollback = value;
				NotifyPropertyChanged("ForceRollback");

				if(_forceRollback)
					NoTransaction = false;

				if (_forceRollback == false)
					CheckFilesOnly = false;
			}
		}

		public bool CheckFilesOnly
		{
			get { return _checkFilesOnly; }
			set
			{
				if (_checkFilesOnly == value)
					return;

				_checkFilesOnly = value;
				NotifyPropertyChanged("CheckFilesOnly");

				if (_checkFilesOnly)
				{	
					BypassCheck = false;
					NoTransaction = false;
					ForceRollback = true;
				}
			}
		}

		public bool BypassCheck
		{
			get { return _bypassCheck; }
			set
			{
				if (_bypassCheck == value)
					return;

				_bypassCheck = value;
				NotifyPropertyChanged("BypassCheck");

				if(_bypassCheck)
					CheckFilesOnly = false;
			}
		}

		public async Task FindServersAsync()
		{
			var defaultItem = new ComboBoxItem {Content = "localhost"};

			try
			{
				var results = await Task.Factory.StartNew(() =>
				{
					var dataTable = SmoApplication.EnumAvailableSqlServers(false);
					return dataTable.Rows.Cast<DataRow>().Select(row => (string) row["Name"]).ToList();
				});

				var serverList = results.Select(name =>
					new ComboBoxItem
					{
						Content = name,
						Foreground = name == Environment.MachineName ? Brushes.SteelBlue : Brushes.Black
					})
					.ToList();

				serverList.Insert(0, defaultItem);
				ServersList = serverList;
			}
			catch
			{
				ServersList = new List<ComboBoxItem> { defaultItem };
			}
		}

		private void NotifyPropertyChanged(params string[] propNames)
		{
			var handler = PropertyChanged;
			if (handler == null)
				return;

			foreach(var prop in propNames)
				handler(this, new PropertyChangedEventArgs(prop));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public static explicit operator DeploymentAppSettings(ViewModel input)
		{
			return new DeploymentAppSettings
			{
				ProjectFolder = input.ProjectFolder,
				LogFolder = input.LogFolder,
				SqlServer = input.SqlServer,
				NoTransaction = input.NoTransaction,
				ForceRollback = input.ForceRollback,
				CheckFilesOnly = input.CheckFilesOnly,
				BypassCheck = input.BypassCheck
			};
		}

		private readonly DelegateCommand<string> _projectFolderCommand, _logFolderCommand;
		private string _projectFolder, _logFolder, _sqlServer;
		private bool _isBusy, _noTransaction, _forceRollback, _checkFilesOnly, _bypassCheck;
		private List<ComboBoxItem> _serversList;
	}
}