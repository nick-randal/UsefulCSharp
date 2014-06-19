/*
Useful C#
Copyright (C) 2014  Nicholas Randal

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Randal.Core.IO.Logging
{
	public interface ILogEntryFormatter
	{
		string Format(ILogEntry entry);
	}

	public sealed class LogEntryFormatter : ILogEntryFormatter
	{
		public LogEntryFormatter(string insetText = null)
		{
			_insetText = insetText ?? "  ";

			_prepends = new List<string>(5)
			{
				string.Empty,
				_insetText,
				string.Join(string.Empty, Enumerable.Repeat(_insetText, 2)),
				string.Join(string.Empty, Enumerable.Repeat(_insetText, 3)),
				string.Join(string.Empty, Enumerable.Repeat(_insetText, 4))
			};
		}

		public string Format(ILogEntry entry)
		{
			var group = entry as ILogGroupEntry;
			if (group != null)
				return Format(group);

			return string.Format("{0} {1}{2}{3}",
						entry.ShowTimestamp ? entry.Timestamp.ToString(TextResources.Timestamp) : TextResources.NoTimestamp,
						StepInText(),
						entry.Message,
						Environment.NewLine
					);
		}

		private string Format(ILogGroupEntry group)
		{
			var text = string.Format("{0} {1}{2}{3}{4}",
							group.ShowTimestamp ? group.Timestamp.ToString(TextResources.Timestamp) : TextResources.NoTimestamp,
							StepInText(),
							group.IsEnd ? TextResources.GroupLeadOut : TextResources.GroupLeadIn,
							group.Message,
							Environment.NewLine
						);

			if (group.IsEnd)
			{
				if (_insetLevel > 0)
					_insetLevel--;
			}
			else
				_insetLevel++;

			return text;
		}

		private string StepInText()
		{
			if (_insetLevel < _prepends.Count) 
				return _prepends[_insetLevel];

			for (var n = _prepends.Count; n <= _insetLevel; n++)
				_prepends.Add(string.Join(string.Empty, Enumerable.Repeat(_insetText, n)));

			return _prepends[_insetLevel];
		}

		private int _insetLevel;
		private readonly string _insetText;
		private readonly List<string> _prepends;
	}
}