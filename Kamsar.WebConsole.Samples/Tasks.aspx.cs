using System.Globalization;
using System.Threading;

namespace Kamsar.WebConsole.Samples
{
    public partial class Tasks : WebConsolePage
	{
		protected override string PageTitle
		{
			get
			{
				return "WebConsole Tasks Demonstration";
			}
		}

		protected override void Process(IProgressStatus progress)
		{
			progress.ReportStatus("Starting WebForms tasks demonstration...");

			const int subtasks = 3;
			for (int i = 1; i <= subtasks; i++)
			{
				using (var subtask = new SubtaskProgressStatus("Demonstration sub-task #" + i.ToString(CultureInfo.InvariantCulture), progress, i, subtasks + 1, false))
				{
					ExecuteTask(subtask);
				}
				progress.ReportStatus("Sub-task {0}/{1} done. Waiting a sec.", i, subtasks);
				Thread.Sleep(1000);
			}

			progress.ReportStatus("Demonstrating nested sub-tasks...");
			
			// you can also nest subtasks 
			// many times this might be used if a method that accepts an IProgressStatus itself calls sub-methods that also take an IProgressStatus
			// methods that accept an IProgressStatus should *ALWAYS* presume that their progress should be reported as 0-100 (ie that they are running in a subtask)

			using (var subtask = new SubtaskProgressStatus("Demonstration parent sub-task", progress, subtasks+1, subtasks + 1))
			{
				using (var innerSubtask = new SubtaskProgressStatus("Inner task 1", subtask, 1, 2))
				{
					ExecuteTask(innerSubtask);
				}

				using (var innerSubtask2 = new SubtaskProgressStatus("Inner task 2", subtask, 2, 2))
				{
					ExecuteTask(innerSubtask2);
				}
			}

			progress.ReportStatus("WebForms tasks demo complete.");
			progress.ReportStatus("Done.");
		}

		protected void ExecuteTask(IProgressStatus progress)
		{
			for (int i = 0; i <= 100; i++)
			{
				// slight delay to see loading time
				Thread.Sleep(10);

				// demonstrate setting a substatus of the progress bar (e.g. "making database backup")
				if (i % 10 == 0)
				{
					progress.ReportTransientStatus(string.Format("{0}/{1}", i, 100));

					// write some stuff to the console to demonstrate detailed output
					progress.ReportStatus("Task percent {0}", MessageType.Info, i);
				}

				// advance the progress bar status (you can use x % as well as x of y total items)
				progress.Report(i);
			}
		}
	}
}