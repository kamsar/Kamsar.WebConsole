using System;
using System.Web.UI;

namespace Kamsar.WebConsole.Samples
{
	public partial class Customized : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var console = new WebConsole(Response);

			// emit JS/CSS
			console.RenderResources();

			Response.Write("<div class=\"wrapper\">");

			// emit console on top
			console.RenderConsole();

			// emit progress at the bottom
			console.RenderProgressBar();

			Response.Write("</div>");

			console.WriteLine("Starting WebForms custom demonstration...");

			// if directly using a WebConsole for rendering, you should treat it as IProgressStatus during the rendering portion
			// to expose consistent progress APIs
			IProgressStatus progress = console;

			for (int i = 0; i <= 100; i++)
			{
				// slight delay to see loading time
				System.Threading.Thread.Sleep(50);

				// advance the progress bar status (you can use x % as well as x of y total items)
				progress.Report(i);

				// demonstrate setting a substatus of the progress bar (e.g. "making database backup")
				if (i % 10 == 0) progress.ReportStatus(string.Format("{0}/{1}", i, 100));

				progress.ReportStatus("At {0}%", console.Progress);
			}

			console.SetTransientStatus("WebForms demo complete.");
		}
	}
}