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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Randal.Sql.Deployer.Shared;
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
			var settings = await DeploymentAppSettings.Load() ?? new DeploymentAppSettings();

			Model = new ViewModel(settings, this.CreateWrapper());
			var findServers = Model.FindServersAsync();

			UpdateStatus();

			StartLogExchangeHost();

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
			using(_host)
				_host.Close();

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
			Output.WriteLine(string.Empty);

			await Task.Delay(1000);

			IProgress<string> progressOutput = new Progress<string>(text => Output.WriteLine(text));
			IProgress<string> progressError = new Progress<string>(text => Output.WriteLine(Brushes.LightPink, text));

			var settings = (DeploymentAppSettings)Model;
			settings.ApplicationPath = DeployerPath;
			if (settings.ApplicationPath != null)
			{
				Output.WriteLine("Deployer: " + settings.ApplicationPath);
				await new ScriptDeploymentProcess().Run(progressOutput, progressError, settings);
			}
			else
				Output.WriteLine("Deployer application not found.");

			UpdateStatus();
		}

		

		private void ProjectFolder_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateBackground(ProjectFolder, Directory.Exists(ProjectFolder.Text));
		}

		private void LogFolder_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateBackground(LogFolder, Directory.Exists(LogFolder.Text));
		}


		private void Output_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var element = Output.InputHitTest(e.MouseDevice.GetPosition(Output)) as Run;
			if (element == null)
				return;

			if (element.TextDecorations.Count(td => td.Location == TextDecorationLocation.Underline) == 0)
				return;

			Output.WriteLine(Brushes.MediumSpringGreen, "-> Opening log...");
			Process.Start(element.Text);
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

		private void StartLogExchangeHost()
		{
			try
			{
				_host = new ServiceHost(typeof (LogExchange), new Uri(SharedConst.NetPipe));

				_host.AddServiceEndpoint(typeof (ILogExchange), new NetNamedPipeBinding(), SharedConst.EndPointBinding);

				_host.Open();
			}
			catch (Exception ex)
			{
				Output.WriteLine(Brushes.LightPink, ex.Message);
			}
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
		private ServiceHost _host;

		private const string
			DeployerExeName = "Randal.Sql.Deployer.App.exe",
			ConfigFileDialogFilter = "Config file|*.cfg",
			DeployerFileDialogFilter = "Deployer App|*.exe"
		;
	}
}
