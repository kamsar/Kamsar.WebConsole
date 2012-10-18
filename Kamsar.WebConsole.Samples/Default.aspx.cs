using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Kamsar.WebConsole.Samples
{
    public partial class Default : WebConsolePage
    {
        protected override string GetPageTitle()
        {
            return "WebConsole Demonstration";
        }

        protected override string GetTaskName()
        {
            return "Reticulating splines, please wait.";
        }


        protected override void Process()
        {
            for (int i = 0; i <= 100; i++)
            {
                // slight delay to see loading time
                System.Threading.Thread.Sleep(50);

                // advance the progress bar status (you can use x % as well as x of y total items)
                SetProgress(i);

                // demonstrate setting a substatus of the progress bar (e.g. "making database backup")
                if (i % 10 == 0) SetProgressStatus(string.Format("{0}/{1}", i, 100));

                // write some stuff to the console to demonstrate detailed output
                WriteConsoleLine("Processed item {0} like a boss.", MessageType.Info, i);
                if (i == 90) WriteConsoleLine("Oops, fake error", MessageType.Error);
                if (i == 91) WriteConsoleLine("Warning: this can be harmful if misused.", MessageType.Warning);
                if (i == 92)
                {
                    WriteConsole("You can also ", MessageType.Info);
                    WriteConsoleLine("mix message types, and {0} {1}", MessageType.Debug, "use", "string formatting");
                }
            }
        }

        
    }
}