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
