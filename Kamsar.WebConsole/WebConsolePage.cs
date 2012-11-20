using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Kamsar.WebConsole
{
    public abstract class WebConsolePage : Page
    {
		WebConsole _console;
		
		protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
			_console = new WebConsole(new HttpResponseWrapper(Response));
        }

        protected abstract void Process(MinimalWebConsole console);
		protected abstract string PageTitle { get; }

		protected override void Render(HtmlTextWriter writer)
		{
			_console.Title = PageTitle;
			_console.Render(Process);
		}

        
    }
}