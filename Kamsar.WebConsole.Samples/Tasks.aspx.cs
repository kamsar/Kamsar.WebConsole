using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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

		protected override void Process(WebConsole console)
		{
			console.WriteLine("Starting WebForms tasks demonstration...");

			int subtasks = 3;
			for (int i = 1; i <= subtasks; i++)
			{
				ExecuteTask(i, subtasks, "Demonstration sub-task #" + i.ToString(), console);
				console.WriteLine("Sub-task {0}/{1} done. Waiting a sec.", i, subtasks);
				Thread.Sleep(1000);
			}

			console.WriteLine("Demonstrating bracketed sub-tasks...");
			ExecuteRangeTask(0, 20, "0-20%", console);
			console.WriteLine("Sub-task 0-20 done. Waiting a sec.");
			Thread.Sleep(1000);
			ExecuteRangeTask(20, 62, "20-62%", console);
			console.WriteLine("Sub-task 20-62 done. Waiting a sec.");
			Thread.Sleep(1000);
			ExecuteRangeTask(62, 98, "62-98%", console);
			console.WriteLine("Sub-task 62-98 done. Waiting a sec.");
			Thread.Sleep(1000);
			ExecuteRangeTask(98, 100, "98-100%", console);
			console.WriteLine("Sub-task 98-100 done. Waiting a sec.");
			Thread.Sleep(1000);

			console.SetProgressStatus("WebForms tasks demo complete.");
			console.WriteLine("Done.");
		}

		protected void ExecuteTask(int index, int total, string taskName, WebConsole console)
		{
			console.WriteLine("Starting {0} task...", taskName);

			for (int i = 0; i <= 100; i++)
			{
				// slight delay to see loading time
				System.Threading.Thread.Sleep(10);

				// advance the progress bar status (you can use x % as well as x of y total items)
				console.SetTaskProgress(index, total, i);

				// demonstrate setting a substatus of the progress bar (e.g. "making database backup")
				if (i % 10 == 0) console.SetProgressStatus(string.Format("{0}: {1}/{2}", taskName, i, 100));

				// write some stuff to the console to demonstrate detailed output
				console.WriteLine("Task percent {0}", MessageType.Info, i);
			}

			console.SetProgressStatus("{0} complete.", taskName);
		}

		protected void ExecuteRangeTask(int startPercent, int endPercent, string taskName, WebConsole console)
		{
			console.WriteLine("Starting {0} task...", taskName);

			for (int i = 0; i <= 100; i++)
			{
				// slight delay to see loading time
				System.Threading.Thread.Sleep(10);

				// advance the progress bar status (you can use x % as well as x of y total items)
				console.SetRangeTaskProgress(startPercent, endPercent, i);

				// demonstrate setting a substatus of the progress bar (e.g. "making database backup")
				if (i % 10 == 0) console.SetProgressStatus(string.Format("{0}: {1}/{2}", taskName, i, 100));

				// write some stuff to the console to demonstrate detailed output
				console.WriteLine("Task percent {0}", MessageType.Info, i);
			}

			console.SetProgressStatus("{0} complete.", taskName);
		}
	}
}