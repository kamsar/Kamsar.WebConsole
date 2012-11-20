using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kamsar.WebConsole.MvcSamples.Controllers
{
    public class ConsoleController : Controller
    {
        public ActionResult Demo()
        {
			var processor = new WebConsole(Response);

			processor.Title = "MVC Demo";

			processor.Render(console =>
			{
				console.WriteLine("WebConsole MVC Demo starting...");

				for (int i = 0; i <= 100; i++)
				{
					// slight delay to see loading time
					System.Threading.Thread.Sleep(50);

					// advance the progress bar status (you can use x % as well as x of y total items)
					console.SetProgress(i);

					// demonstrate setting a substatus of the progress bar (e.g. "making database backup")
					if (i % 10 == 0) console.SetProgressStatus(string.Format("{0}/{1}", i, 100));

					// write some stuff to the console to demonstrate detailed output
					console.WriteLine("Processed item {0} like a boss.", MessageType.Info, i);
					if (i == 90) console.WriteLine("Oops, fake error", MessageType.Error);
					if (i == 91) console.WriteLine("Warning: this can be harmful if misused.", MessageType.Warning);
					if (i == 92)
					{
						console.Write("You can also ", MessageType.Info);
						console.WriteLine("mix message types, and {0} {1}", MessageType.Debug, "use", "string formatting");
					}
				}

				console.SetProgressStatus("Completed MVC demo.");
			});

			return Content("");
        }
    }
}
