using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kamsar.WebConsole.MvcSamples.Models;

namespace Kamsar.WebConsole.MvcSamples.Controllers
{
    public class ConsoleController : Controller
    {
        public ActionResult Demo()
        {
			var processor = new Html5WebConsole(Response);

			processor.Title = "MVC Demo";

			processor.Render(progress =>
			{
				progress.ReportStatus("WebConsole MVC Demo starting...");

				for (int i = 0; i <= 100; i++)
				{
					// slight delay to see loading time
					System.Threading.Thread.Sleep(50);

					// advance the progress bar status (you can use x % as well as x of y total items)
					progress.Report(i);

					// demonstrate setting a substatus of the progress bar (e.g. "making database backup")
					if (i % 10 == 0) progress.ReportTransientStatus(string.Format("{0}/{1}", i, 100));

					// write some stuff to the console to demonstrate detailed output
					progress.ReportStatus("At {0}", MessageType.Info, i);
					if (i == 90) progress.ReportStatus("Oops, fake error", MessageType.Error);
					if (i == 91) progress.ReportStatus("Warning: this can be harmful if misused.", MessageType.Warning);
					if (i == 92)
					{
						progress.ReportStatus("You can also {0} {1}", MessageType.Debug, "use", "string formatting");
					}

					if (i == 95)
					{
						progress.ReportStatus("I'm about to throw an exception and write its data to the console!");

						// code that can throw an exception should have it caught and written to the console
						// normally you might wrap the whole processing in a try-catch block
						try
						{
							throw new BadJokeException("I'm giving it all she's got Jim!", new Exception("Warp core breach"));
						}
						catch (Exception ex)
						{
							progress.ReportException(ex);
						}
					}
				}

				progress.ReportStatus("Completed MVC demo.");
			});

			return Content("");
        }
    }
}
