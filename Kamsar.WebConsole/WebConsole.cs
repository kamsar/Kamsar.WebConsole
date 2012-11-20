using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Kamsar.WebConsole
{
	public class WebConsole : MinimalWebConsole
	{
		HttpResponseBase _response;

		public WebConsole(HttpResponseBase response) : base(response)
		{
			_response = response;
			Title = string.Empty;
		}

		public string Title { get; set; }

        protected virtual void RenderHead()
        {
			if (!string.IsNullOrEmpty(Title))
			{
				_response.Write(string.Format("<title>{0}</title>", Title));
			}

			RenderResources();
        }

		public override void Render(Action<MinimalWebConsole> processAction)
        {
			_response.Write("<!DOCTYPE html>");

			_response.Write("<html>");
			_response.Write("<head>");

            RenderHead();

			_response.Write("</head>");
			_response.Write("<body>");
			_response.Write("<div class=\"wrapper\">");

			if (!string.IsNullOrEmpty(Title))
			{
				_response.Write(string.Format("<h1>{0}</h1>", Title));
			}

            RenderProgressBar();
            RenderConsole();

			_response.Write("</div>");
			_response.Flush();

			processAction(this);

			_response.Write("</body>");
			_response.Write("</html>");
			_response.Flush();
        }
	}
}
