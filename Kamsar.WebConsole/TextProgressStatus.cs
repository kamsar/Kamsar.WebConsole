using System;
using System.Diagnostics;
using System.Text;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// Implementation of IProgressStatus that captures the output of the progress reports as a string log
	/// </summary>
	public class TextProgressStatus : IProgressStatus
	{
		readonly StringBuilder _console = new StringBuilder();

		public void Report(int percent)
		{
			// do nothing, we don't care about percents here
		}

		public void Report(int percent, string statusMessage)
		{
			ReportStatus(statusMessage, MessageType.Info);
		}

		public void Report(int percent, string statusMessage, MessageType type)
		{
			ReportStatus(statusMessage, type);
		}

		public void ReportStatus(string statusMessage)
		{
			ReportStatus(statusMessage, MessageType.Info);
		}

		public void ReportStatus(string statusMessage, MessageType type)
		{
			_console.AppendFormat("{0}: {1} ", type, statusMessage);
		}

		public void ReportException(Exception exception)
		{
			var exMessage = new StringBuilder();
			exMessage.AppendFormat("{0} ({1})", exception.Message, exception.GetType().FullName);

			if (Debugger.IsAttached) Debugger.Break();

			ReportStatus(exMessage.ToString(), MessageType.Error);
		}

		public string Output { get { return _console.ToString(); } }
	}
}
