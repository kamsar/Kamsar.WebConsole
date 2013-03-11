using System;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// Generic progress reporting interface that can be passed as a parameter to methods that need to report progress
	/// </summary>
	public interface IProgressStatus
	{
		/// <summary>
		/// Reports the current task's completion status
		/// </summary>
		/// <param name="percent">Task completion in percent</param>
		void Report(int percent);

		/// <summary>
		/// The current progress, in percent, of this progress status instance
		/// </summary>
		int Progress { get; }

		/// <summary>
		/// Reports an exception occurred while running the task.
		/// </summary>
		void ReportException(Exception exception);

		/// <summary>
		/// Reports a informational status log entry
		/// </summary>
		/// <param name="statusMessage">The message to report</param>
		/// <param name="formatParameters">Format parameters for the statusMessage parameter (string.format)</param>
		void ReportStatus(string statusMessage, params object[] formatParameters);

		/// <summary>
		/// Reports a informational status log entry
		/// </summary>
		/// <param name="statusMessage">The message to report</param>
		/// <param name="type">The type of message to report</param>
		/// <param name="formatParameters">Format parameters for the statusMessage parameter (string.format)</param>
		void ReportStatus(string statusMessage, MessageType type, params object[] formatParameters);
		
		/// <summary>
		/// Reports a transient status (i.e. "processed 27/40") that is displayed until changed but not logged
		/// </summary>
		/// <param name="statusMessage">The message to report</param>
		/// <param name="formatParameters">Format parameters for the statusMessage parameter (string.format)</param>
		void ReportTransientStatus(string statusMessage, params object[] formatParameters);
	}
}
