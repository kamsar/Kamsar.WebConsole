using System;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// Variant of the IProgressStatus that saves the output lines to a StringBuilder for programmatic capture
	/// 
	/// Progress reports are not captured.
	/// </summary>
	public class StringProgressStatus : IProgressStatus
	{
		readonly StringBuilder _output = new StringBuilder();
		readonly StringBuilder _errors = new StringBuilder();
		readonly StringBuilder _warnings = new StringBuilder();
		int _progressPercent;

		public void ReportException(Exception exception)
		{
			var exMessage = new StringBuilder();
			exMessage.AppendFormat("ERROR: {0} ({1})", exception.Message, exception.GetType().FullName);
			exMessage.AppendLine();

			if (exception.StackTrace != null)
				exMessage.Append(exception.StackTrace.Trim());
			else
				exMessage.Append("No stack trace available.");

			exMessage.AppendLine();

			WriteInnerException(exception.InnerException, exMessage);

			ReportStatus(exMessage.ToString(), MessageType.Error);

			if (Debugger.IsAttached) Debugger.Break();
		}

		public void ReportStatus(string statusMessage, params object[] formatParameters)
		{
			ReportStatus(statusMessage, MessageType.Info, formatParameters);
		}

		public void ReportStatus(string statusMessage, MessageType type, params object[] formatParameters)
		{
			var line = new StringBuilder();

			line.AppendFormat("{0}: ", type);

			if (formatParameters.Length > 0)
				line.AppendFormat(statusMessage, formatParameters);
			else
				line.Append(statusMessage);

			_output.AppendLine(HttpUtility.HtmlEncode(line.ToString()));

			if (type == MessageType.Error)
				_errors.AppendLine(HttpUtility.HtmlEncode(line.ToString()));

			if (type == MessageType.Warning)
				_warnings.AppendLine(HttpUtility.HtmlEncode(line.ToString()));
		}
		
		public void Report(int percent)
		{
			_progressPercent = percent;
		}

		public void ReportTransientStatus(string statusMessage, params object[] formatParameters)
		{
			// do nothing
		}

		/// <summary>
		/// All available console output
		/// </summary>
		public string Output { get { return _output.ToString(); } }

		/// <summary>
		/// All error output from the console
		/// </summary>
		public string Errors { get { return _errors.ToString(); } }

		/// <summary>
		/// All warning output from the console
		/// </summary>
		public string Warnings { get { return _warnings.ToString(); } }

		public bool HasErrors { get { return _errors.Length > 0; } }
		public bool HasWarnings { get { return _warnings.Length > 0; } }

		public int Progress { get { return _progressPercent; } }

		private static void WriteInnerException(Exception innerException, StringBuilder exMessage)
		{
			if (innerException == null) return;

			exMessage.AppendLine("INNER EXCEPTION");
			exMessage.AppendFormat("{0} ({1})", innerException.Message, innerException.GetType().FullName);
			exMessage.AppendLine();

			if (innerException.StackTrace != null)
				exMessage.Append(innerException.StackTrace.Trim());
			else
				exMessage.Append("No stack trace available.");

			WriteInnerException(innerException.InnerException, exMessage);

			exMessage.AppendLine();
		}
	}
}
