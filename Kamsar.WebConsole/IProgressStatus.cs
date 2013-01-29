using System;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// Generic progress reporting interface that can be passed as a parameter to methods that need to report progress
	/// </summary>
	public interface IProgressStatus
	{
		void Report(int percent);
		void Report(int percent, string statusMessage);
		void Report(int percent, string statusMessage, MessageType type);
		void ReportException(Exception exception);
		void ReportStatus(string statusMessage);
		void ReportStatus(string statusMessage, MessageType type);
	}
}
