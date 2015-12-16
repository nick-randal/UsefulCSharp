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
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text;

namespace Randal.Logging
{
	public interface ILogEntryFormatter
	{
		string Format(ILogEntry entry);
	}

	public sealed class LogEntryFormatter : ILogEntryFormatter
	{
		public string Format(ILogEntry entry)
		{
			if (entry is LogExceptionEntry)
			{
				return FormatException((LogExceptionEntry)entry);
			}

			return string.Concat(
				entry.ShowTimestamp ? entry.Timestamp.ToString(TextResources.Timestamp) : TextResources.NoTimestamp,
				TextResources.Prepend,
				entry.Message,
				Environment.NewLine
			);
		}

		private static string FormatException(LogExceptionEntry entry)
		{
			var text = new StringBuilder();

			if (string.IsNullOrWhiteSpace(entry.Message) == false)
				text.AppendLine(entry.Message.Trim());

			if (entry.Exception == null)
			{
				text.AppendLine(TextResources.Errors.NoExceptionProvided);
				return text.ToString();
			}

			text.AppendLine(TextResources.Errors.ErrorInfo);
			text.AppendLine(TextResources.DashedBreak80Wide);

			var inner = entry.Exception;
			while (inner != null)
			{
				try
				{
					if (FormatSqlException(text, inner as SqlException) ||
					    FormatSmtpException(text, inner as SmtpException))
					{
					}
					else
						text.AppendFormat("{0} : {1}", inner.GetType().FullName, inner.Message);
				}
				finally
				{
					text.AppendLine();
					inner = inner.InnerException;
				}
			}

			FormatStackTrace(entry, text);

			return text.ToString();
		}

		private static void FormatStackTrace(LogExceptionEntry entry, StringBuilder text)
		{
			if (entry.Exception == null)
				return;

			text.AppendLine();
			text.AppendLine("Stack Trace");
			text.AppendLine();
			text.AppendLine(entry.Exception.StackTrace);
			text.AppendLine(TextResources.DashedBreak80Wide);
		}

		private static bool FormatSmtpException(StringBuilder text, SmtpException smtpEx)
		{
			if (smtpEx == null)
				return false;

			text.AppendFormat("SMTP Error : {0} - {1}", smtpEx.StatusCode, smtpEx.Message);
			return true;
		}

		private static bool FormatSqlException(StringBuilder text, SqlException sqlEx)
		{
			if (sqlEx == null)
				return false;

			text.AppendLine(sqlEx.Message);
			text.AppendFormat("Source {0}, Code {1}, Line {2}, Procedure {3}", sqlEx.Source, sqlEx.Number, sqlEx.LineNumber,
				sqlEx.Procedure);
			text.AppendLine();

			for (var i = 0; i < sqlEx.Errors.Count; i++)
			{
				text.AppendFormat("{0} line {1}: {2}",
					sqlEx.Errors[i].Procedure, sqlEx.Errors[i].LineNumber, sqlEx.Errors[i].Message
					);
			}

			return true;
		}
	}
}