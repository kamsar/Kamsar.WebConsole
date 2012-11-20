using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Kamsar.WebConsole.Samples
{
	public partial class Customized : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var processor = new MinimalWebConsole(new HttpResponseWrapper(Response));

			// emit JS/CSS
			processor.RenderResources();

			Response.Write("<div class=\"wrapper\">");

			// emit console on top
			processor.RenderConsole();

			// emit progress at the bottom
			processor.RenderProgressBar();

			Response.Write("</div>");

			processor.Render(console =>
			{
				console.WriteLine("Starting WebForms custom demonstration...");

				for (int i = 0; i <= 100; i++)
				{
					// slight delay to see loading time
					System.Threading.Thread.Sleep(50);

					// advance the progress bar status (you can use x % as well as x of y total items)
					console.SetProgress(i);

					// demonstrate setting a substatus of the progress bar (e.g. "making database backup")
					if (i % 10 == 0) console.SetProgressStatus(string.Format("{0}/{1}", i, 100));

					console.WriteLine("At {0}%", console.Progress);
				}

				console.SetProgressStatus("WebForms demo complete.");
			});
		}
	}
}