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

using System.ComponentModel;

namespace Randal.Sql.Deployer.UI
{
	public sealed class ViewModel : INotifyPropertyChanged
	{
		public ViewModel()
		{
			IsBusy = false;
		}

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

		private void NotifyPropertyChanged(params string[] propNames)
		{
			var handler = PropertyChanged;
			if (handler == null)
				return;

			foreach(var prop in propNames)
				handler(this, new PropertyChangedEventArgs(prop));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public static explicit operator ViewModel(DeploymentAppSettings input)
		{
			return new ViewModel
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

		private string _projectFolder, _logFolder, _sqlServer;
		private bool _isBusy, _noTransaction, _forceRollback, _checkFilesOnly, _bypassCheck;
	}
}