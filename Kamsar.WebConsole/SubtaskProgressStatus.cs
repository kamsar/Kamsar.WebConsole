using System;
using System.Globalization;
using System.Timers;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// An implementation of IProgressStatus designed to be used as a "subtask" within a parent IProgressStatus.
	/// This encapsulates the idea of task progress, e.g. the method receiving this progress object
	/// need only be concerned with its own 1-100% progress, not the overall progress of all tasks.
	/// The total progress of the console is scaled proprotionally to the number of total tasks provided.
	/// </summary>
	/// <example>
	/// When using this class, use it as an IDisposable, eg
	/// using(var progress = new SubtaskProgressStatus("Doing something or other", console))
	/// {
	///		DoSomeMethod(progress);
	/// }
	/// 
	/// This allows it to time the execution of the task.
	/// </example>
	public class SubtaskProgressStatus : IProgressStatus, IDisposable
	{
		private readonly IProgressStatus _mainTask;
		private readonly int _subtaskIndex;
		private readonly int _subtaskCount;
		private readonly bool _automaticTransientStatus;
		private Timer _heartbeat;
		private readonly DateTime _startTime = DateTime.Now;
		private readonly string _taskName;
		private int _progress;

		/// <summary>
		/// Default constructor. Automatically manages transient status for you.
		/// </summary>
		/// <param name="taskName">Name of the subtask to run</param>
		/// <param name="mainTask">The progress status of the main task (or parent subtask)</param>
		/// <param name="subtaskIndex">The index of this subtask among total subtask count (used to calculate offset progress)</param>
		/// <param name="subtaskCount">The total number of subtasks in the main task (or parent subtask)</param>
		public SubtaskProgressStatus(string taskName, IProgressStatus mainTask, int subtaskIndex, int subtaskCount)
			: this(taskName, mainTask, subtaskIndex, subtaskCount, true)
		{
			
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="taskName">Name of the subtask to run</param>
		/// <param name="mainTask">The progress status of the main task (or parent subtask)</param>
		/// <param name="subtaskIndex">The index of this subtask among total subtask count (used to calculate offset progress)</param>
		/// <param name="subtaskCount">The total number of subtasks in the main task (or parent subtask)</param>
		/// <param name="automaticTransientStatus">If true, transient status will be automatically managed for you for this subtask ($taskname running for xx seconds). Make sure to dispose the subtask!</param>
		public SubtaskProgressStatus(string taskName, IProgressStatus mainTask, int subtaskIndex, int subtaskCount, bool automaticTransientStatus)
		{
			_subtaskIndex = subtaskIndex;
			_subtaskCount = subtaskCount;

			_mainTask = mainTask;
			_taskName = taskName;
			_automaticTransientStatus = automaticTransientStatus;

			InitializeStatus();;
		}

		public virtual void Report(int percent)
		{
			SetProgress(percent);
		}

		public virtual void Report(int percent, string statusMessage, params object[] formatParameters)
		{
			Report(percent, statusMessage, MessageType.Info, formatParameters);
		}

		public virtual void Report(int percent, string statusMessage, MessageType type, params object[] formatParameters)
		{
			SetProgress(percent);
			_mainTask.ReportStatus(statusMessage, type, formatParameters);
		}

		public virtual void ReportStatus(string statusMessage, params object[] formatParameters)
		{
			ReportStatus(statusMessage, MessageType.Info, formatParameters);
		}

		public virtual void ReportStatus(string statusMessage, MessageType type, params object[] formatParameters)
		{
			_mainTask.ReportStatus(statusMessage, type, formatParameters);
		}

		public virtual void ReportException(Exception exception)
		{
			_mainTask.ReportException(exception);
		}

		public virtual void Dispose()
		{
			if (_heartbeat != null)
			{
				_heartbeat.Stop();
				_heartbeat.Dispose();
			}

			if(_automaticTransientStatus)
				ReportTransientStatus(string.Empty);

			if(Progress < 100)
				Report(100);

			_mainTask.ReportStatus("{0} has completed in  {1} sec", MessageType.Debug, _taskName, Math.Round((DateTime.Now - _startTime).TotalSeconds).ToString(CultureInfo.InvariantCulture));
		}

		public virtual int Progress => _progress;

		protected virtual void SetProgress(int taskProgress)
		{
			_progress = taskProgress;
			SetTaskProgress(_subtaskIndex, _subtaskCount, taskProgress);
		}

		protected virtual void InitializeStatus()
		{
			if (!_automaticTransientStatus) return;

			ReportTransientStatus(_taskName + " running");

			_heartbeat = new Timer(2000);
			_heartbeat.AutoReset = true;
			_heartbeat.Elapsed += (sender, args) =>
			{
				var elapsed = Math.Round((args.SignalTime - _startTime).TotalSeconds);

				ReportTransientStatus("{0} running ({1} sec)", _taskName, elapsed.ToString(CultureInfo.InvariantCulture));
			};

			_heartbeat.Start();
		}

		/// <summary>
		/// Sets the progress of the whole based on the progress within a sub-task of the main progress (e.g. 0-100% of a task within the global range of 0-20%)
		/// </summary>
		/// <param name="taskNumber">The index of the current sub-task</param>
		/// <param name="totalTasks">The total number of sub-tasks</param>
		/// <param name="taskPercent">The percentage complete of the sub-task (0-100)</param>
		protected virtual void SetTaskProgress(int taskNumber, int totalTasks, int taskPercent)
		{
			if (taskNumber < 1) throw new ArgumentException("taskNumber must be 1 or more");
			if (totalTasks < 1) throw new ArgumentException("totalTasks must be 1 or more");
			if (taskNumber > totalTasks) throw new ArgumentException("taskNumber was greater than the number of totalTasks!");

			int start = (int)Math.Round(((taskNumber - 1) / (double)totalTasks) * 100d);
			int end = start + (int)Math.Round((1d / totalTasks) * 100d);

			SetRangeTaskProgress(Math.Max(start, 0), Math.Min(end, 100), taskPercent);
		}

		/// <summary>
		/// Sets the progress of the whole based on the progress within a percentage range of the main progress (e.g. 0-100% of a task within the global range of 0-20%)
		/// </summary>
		/// <param name="startPercentage">The percentage the task began at</param>
		/// <param name="endPercentage">The percentage the task ends at</param>
		/// <param name="taskPercent">The percentage complete of the sub-task (0-100)</param>
		protected virtual void SetRangeTaskProgress(int startPercentage, int endPercentage, int taskPercent)
		{
			int range = endPercentage - startPercentage;

			if (range <= 0) throw new ArgumentException("endPercentage must be greater than startPercentage");

			int offset = (int)Math.Round(range * (taskPercent / 100d));

			_mainTask.Report(Math.Min(startPercentage + offset, 100));
		}

		public virtual void ReportTransientStatus(string statusMessage, params object[] formatParameters)
		{
			_mainTask.ReportTransientStatus(statusMessage, formatParameters);
		}
	}
}
