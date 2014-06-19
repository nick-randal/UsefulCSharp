﻿/*
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
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text;

namespace Randal.Core.IO.Logging
{
	public sealed class ExceptionEntry : LogEntry
	{
		public ExceptionEntry(Exception ex, string message = null)
			: base(null, DateTime.Now, Verbosity.Vital)
		{
			FormatException(ex, message);
		}

		private void FormatException(Exception ex, string message)
		{
			if (ex == null)
			{
				Message = TextResources.Errors.NoExceptionProvided;
				return;
			}

			var text = new StringBuilder();

			if (string.IsNullOrWhiteSpace(message) == false)
				text.AppendLine(message.Trim());

			text.AppendLine(TextResources.Errors.ErrorInfo);
			text.AppendLine(TextResources.DashedBreak80Wide);

			var inner = ex;
			while (inner != null)
			{
				try
				{
					if (FormatSqlException(text, inner as SqlException) ||
						FormatSmtpException(text, inner as SmtpException)) {}
					else
						text.AppendFormat("{0} : {1}", ex.GetType().FullName, ex.Message);
				}
				finally
				{
					text.AppendLine();
					inner = inner.InnerException;
				}
			}

			FormatStackTrace(ex, text);

			Message = text.ToString();
		}

		private static void FormatStackTrace(Exception ex, StringBuilder text)
		{
			text.AppendLine();
			text.AppendLine("Stack Trace");
			text.AppendLine();
			text.AppendLine(ex.StackTrace);
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
			text.AppendFormat("Source {0}, Code {1}, Line {2}, Procedure {3}", sqlEx.Source, sqlEx.Number, sqlEx.LineNumber, sqlEx.Procedure);
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