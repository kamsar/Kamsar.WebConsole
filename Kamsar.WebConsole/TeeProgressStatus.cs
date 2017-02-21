using System;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// "Splits" a progress status to multiple destination progress objects
	/// This is useful when you may want to send progress output both to a file log and a webconsole,
	/// or other multi-destination tasks. 
	/// </summary>
	public class TeeProgressStatus : IProgressStatus
	{
		private readonly IProgressStatus[] _progressReporters;

		public TeeProgressStatus(params IProgressStatus[] progressReporters)
		{
			if(progressReporters == null) throw new ArgumentNullException(nameof(progressReporters));
			if(progressReporters.Length == 0) throw new ArgumentOutOfRangeException(nameof(progressReporters), "Must have at least one progress to report to.");

			_progressReporters = progressReporters;
		}

		public void Report(int percent)
		{
			foreach(var reporter in _progressReporters)
				reporter.Report(percent);
		}

		public int Progress
		{
			get { return _progressReporters[0].Progress; }
		}

		public void ReportException(Exception exception)
		{
			foreach (var reporter in _progressReporters)
				reporter.ReportException(exception);
		}

		public void ReportStatus(string statusMessage, params object[] formatParameters)
		{
			foreach (var reporter in _progressReporters)
				reporter.ReportStatus(statusMessage, formatParameters);
		}

		public void ReportStatus(string statusMessage, MessageType type, params object[] formatParameters)
		{
			foreach (var reporter in _progressReporters)
				reporter.ReportStatus(statusMessage, type, formatParameters);
		}

		public void ReportTransientStatus(string statusMessage, params object[] formatParameters)
		{
			foreach (var reporter in _progressReporters)
				reporter.ReportTransientStatus(statusMessage, formatParameters);
		}
	}
}
