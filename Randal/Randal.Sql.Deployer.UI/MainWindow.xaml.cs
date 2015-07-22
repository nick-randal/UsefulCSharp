// Useful C#
// Copyright (C) 2014-2015 Nicholas Randal
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
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Randal.Sql.Deployer.UI.Support;

namespace Randal.Sql.Deployer.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public static RoutedCommand
			ProjectFolderCommand = new RoutedCommand(),
			LogFolderCommand = new RoutedCommand();

		public MainWindow()
		{
			InitializeComponent();
		}

		private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			var settings = await DeploymentAppSettings.Load();
			if (settings == null)
			{
				MessageBox.Show(this, "Failed to load configuration. Config.json file not found.");
				Close();
				return;
			}

			Model = new ViewModel(settings, this.CreateWrapper());
			var findServers = Model.FindServersAsync();

			UpdateStatus();

			await findServers;
		}

		private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			if (Model == null)
				return;

			if (Model.IsBusy)
			{
				e.Cancel = true;
				return;
			}

			UpdateStatus("Saving...");
			await ((DeploymentAppSettings)Model).Save().ConfigureAwait(false);
		}
		
		private void Window_OnClose(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

		private async void Window_Save(object sender, ExecutedRoutedEventArgs e)
		{
			var dialog = new SaveFileDialog
			{
				InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
				Filter = ConfigFileDialogFilter
			};
			if (dialog.ShowDialog(this) == false)
				return;

			await ((DeploymentAppSettings)Model).Save(dialog.FileName).ConfigureAwait(false);
		}

		private async void Window_LoadSettings(object sender, ExecutedRoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
				Filter = ConfigFileDialogFilter
			};
			if (dialog.ShowDialog(this) == false)
				return;

			var settings = await DeploymentAppSettings.Load(dialog.FileName);
			Model = new ViewModel(settings, this.CreateWrapper());
		}

		private async void DeployButton_OnClick(object sender, RoutedEventArgs e)
		{
			UpdateStatus("Deploying...", true);

			using (var task = Task.Factory.StartNew(() => Thread.Sleep(10000)))
			{
				await task;
			}

			IProgress<string> progressOutput = new Progress<string>(text => LogLine(text));
			IProgress<string> progressError = new Progress<string>(text => LogErrorLine(text));

			var settings = (DeploymentAppSettings)Model;
			settings.ApplicationPath = DeployerPath;
			if (settings.ApplicationPath != null)
			{
				await new ScriptDeploymentProcess().Run(progressOutput, progressError, settings);
			}
			else
				LogLine("Deployer application not found.");

			UpdateStatus();
		}

		private void LogLine(string message, params object[] values)
		{
			Output.Inlines.Add(new Run(string.Format(message, values)));
			Output.Inlines.Add(new LineBreak());
		}

		private void LogErrorLine(string message, params object[] values)
		{
			Output.Inlines.Add(new Run(string.Format(message, values)) { Foreground = Brushes.LightPink });
			Output.Inlines.Add(new LineBreak());
		}

		private void ProjectFolder_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateBackground(ProjectFolder, Directory.Exists(ProjectFolder.Text));
		}

		private void LogFolder_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateBackground(LogFolder, Directory.Exists(LogFolder.Text));
		}

		private static void UpdateBackground(Control control, bool isValid)
		{
			control.Background = isValid ? Brushes.White : Brushes.LightPink;
		}

		private void UpdateStatus(string text = "", bool isBusy = false)
		{
			Model.IsBusy = isBusy;
			Status.Content = text;
		}

		public string DeployerPath
		{
			get
			{
				if (File.Exists(_deployerPath))
					return _deployerPath;

				var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DeployerExeName);
				if (File.Exists(path))
				{
					_deployerPath = path;
					return _deployerPath;
				}

				var dialog = new OpenFileDialog
				{
					Title = "Deployer Executable",
					CheckFileExists = true,
					CheckPathExists = true,
					Filter = DeployerFileDialogFilter
				};

				if (!dialog.ShowDialog(this).GetValueOrDefault()) 
					return null;

				_deployerPath = dialog.FileName;
				return _deployerPath;
			}
		}

		public ViewModel Model
		{
			get { return (ViewModel)DataContext; }
			set { DataContext = value; }
		}

		private string _deployerPath;

		private const string
			DeployerExeName = "Randal.Sql.Deployer.App.exe",
			ConfigFileDialogFilter = "Config file|*.cfg",
			DeployerFileDialogFilter = "Deployer App|*.exe"
		;
	}
}
