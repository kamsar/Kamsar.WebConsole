using System;
using System.Web.UI;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// Implements a WebForms page that has a WebConsole as its contents. Content from the page's aspx will be ignored and replaced with the web console.
	/// </summary>
    public abstract class WebConsolePage : Page
    {
		Html5WebConsole _console;
		
		protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
			_console = new Html5WebConsole(Response);
        }

        protected abstract void Process(WebConsole console);
		protected abstract string PageTitle { get; }

		protected override void Render(HtmlTextWriter writer)
		{
			_console.Title = PageTitle;
			_console.Render(Process);
		}  
    }
}