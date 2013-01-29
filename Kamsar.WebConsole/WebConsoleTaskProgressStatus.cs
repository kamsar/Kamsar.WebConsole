using System;
using System.Globalization;
using System.Timers;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// An implementation of IProgressStatus designed to be used with a WebConsole.
	/// This encapsulates the idea of task progress, e.g. the method receiving this progress object
	/// need only be concerned with its own 1-100% progress, not the overall progress of all tasks.
	/// The total progress of the console is scaled proprotionally to the number of total tasks provided.
	/// </summary>
	/// <example>
	/// When using this class, use it as an IDisposable, eg
	/// using(var progress = new WebConsoleTaskProgressStatus("Doing something or other", console))
	/// {
	///		DoSomeMethod(progress);
	/// }
	/// 
	/// This allows it to time the execution of the task.
	/// </example>
	public class WebConsoleTaskProgressStatus : IProgressStatus, IDisposable
	{
		readonly WebConsole _console;
		readonly int _subtaskIndex;
		readonly int _subtaskCount = 100;
		Timer _heartbeat;
		DateTime _startTime;
		readonly string _taskName;

		public WebConsoleTaskProgressStatus(string taskName, WebConsole console)
		{
			_console = console;
			_taskName = taskName;

			InitializeStatus();
		}	

		public WebConsoleTaskProgressStatus(string taskName, WebConsole console, int subtaskIndex, int subtaskCount) : this(taskName, console)
		{
			_subtaskIndex = subtaskIndex;
			_subtaskCount = subtaskCount;
		}

		public void Report(int percent)
		{
			SetProgress(percent);
		}

		public void Report(int percent, string statusMessage)
		{
			Report(percent, statusMessage, MessageType.Info);
		}

		public void Report(int percent, string statusMessage, MessageType type)
		{
			SetProgress(percent);
			_console.WriteLine(statusMessage, type);
		}

		public void ReportStatus(string statusMessage)
		{
			ReportStatus(statusMessage, MessageType.Info);
		}

		public void ReportStatus(string statusMessage, MessageType type)
		{
			_console.WriteLine(statusMessage, type);
		}

		public void ReportException(Exception exception)
		{
			_console.WriteException(exception);
		}

		public void Dispose()
		{
			SetProgress(100);
		}

		private void SetProgress(int taskProgress)
		{
			_console.SetTaskProgress(_subtaskIndex, _subtaskCount, taskProgress);
			if (taskProgress == 100)
			{
				_heartbeat.Stop();

				_console.SetProgressStatus(string.Empty);
				_console.WriteLine("{0} has completed in  {1} sec", MessageType.Debug, _taskName, Math.Round((DateTime.Now - _startTime).TotalSeconds).ToString(CultureInfo.InvariantCulture));

				_heartbeat.Dispose();
			}
		}

		private void InitializeStatus()
		{
			_console.SetProgressStatus(_taskName + " running");

			_startTime = DateTime.Now;
			_heartbeat = new Timer(2000);
			_heartbeat.AutoReset = true;
			_heartbeat.Elapsed += (sender, args) =>
			{
				var elapsed = Math.Round((args.SignalTime - _startTime).TotalSeconds);

				_console.SetProgressStatus("{0} running ({1} sec)", _taskName, elapsed.ToString(CultureInfo.InvariantCulture));
			};
			_heartbeat.Start();
		}
	}
}
