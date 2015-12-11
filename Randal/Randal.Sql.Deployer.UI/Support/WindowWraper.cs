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
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Randal.Sql.Deployer.UI.Support
{
	public interface IWindowWrapper
	{
		bool ShowFolderDialog(string title, string path, out string selectedPath);
	}

	public sealed class WindowWraper : IWindowWrapper, System.Windows.Forms.IWin32Window
	{
		private readonly Window _window;

		public WindowWraper(Window window)
		{
			_window = window;
		}

		public bool ShowFolderDialog(string title, string path, out string selectedPath)
		{
			var dialog = new FolderBrowserDialog
			{
				Description = title,
				SelectedPath = path,
				ShowNewFolderButton = true,
				RootFolder = Environment.SpecialFolder.MyComputer
			};

			var result = dialog.ShowDialog(this) == DialogResult.OK;
			selectedPath = result ? dialog.SelectedPath : null;
			return result;
		}

		public IntPtr Handle
		{
			get { return new WindowInteropHelper(_window).Handle; }
		}
	}
}
