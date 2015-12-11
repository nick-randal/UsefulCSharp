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

namespace Randal.Sql.Deployer.UI.Support
{
	public class DelegateCommand<T> : System.Windows.Input.ICommand
	{
		private readonly Predicate<T> _canExecute;
		private readonly Action<T> _execute;

		public DelegateCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			if (_canExecute == null)
				return true;

			return _canExecute(parameter == null ? default(T) : (T)Convert.ChangeType(parameter, typeof(T)));
		}

		public void Execute(object parameter)
		{
			_execute((parameter == null) ? default(T) : (T)Convert.ChangeType(parameter, typeof(T)));
		}

		public event EventHandler CanExecuteChanged;
		public void RaiseCanExecuteChanged()
		{
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, EventArgs.Empty);
		}
	}
}