using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Kamsar.WebConsole
{
	public class MinimalWebConsole
	{
		private HttpResponseBase _response;
		bool _resourcesRendered = false;
		bool _progressRendered = false;
		bool _consoleRendered = false;

		public MinimalWebConsole(HttpResponseBase response)
		{
			_response = response;
			_response.Buffer = false;
			_response.BufferOutput = false;

			ExtraMessagePaddingLength = 128;
		}

        private int _progress = 0;

		public void RenderResources()
		{
			var page = new Page();
			_response.Write(string.Format("<link rel=\"stylesheet\" href=\"{0}\" />", page.ClientScript.GetWebResourceUrl(typeof(WebConsolePage), "Kamsar.WebConsole.Resources.console.css")));
			_response.Write(string.Format("<script src=\"{0}\"></script>", page.ClientScript.GetWebResourceUrl(typeof(WebConsolePage), "Kamsar.WebConsole.Resources.console.js")));

			_resourcesRendered = true;
		}

        public virtual void RenderProgressBar()
        {
			_response.Write("<section class=\"progress\">");
			_response.Write("<div class=\"progressbar\">");
			_response.Write("<span id=\"percentage\">0%</span>");
			_response.Write("<div id=\"bar\"></div>");
			_response.Write("</div>");
			_response.Write("<p id=\"status\"></p>");
			_response.Write("</section>");

			_progressRendered = true;
        }

        public virtual void RenderConsole()
        {
			_response.Write("<section id=\"console\">");
			_response.Write("</section>");

			_consoleRendered = true;
        }

		public virtual void Render(Action<MinimalWebConsole> processAction)
        {
			// these rendered flags allow you to decide where you want these components in source by invoking their renderings prior to the main render
			if (!_resourcesRendered) RenderResources();
            if (!_progressRendered) RenderProgressBar();
            if (!_consoleRendered) RenderConsole();

			_response.Flush();

            processAction(this);

			_response.Flush();
        }

		public void Write(string statusMessage, params object[] formatParameters)
        {
            Write(statusMessage, MessageType.Info, formatParameters);
        }

		public void Write(string statusMessage, MessageType type, params object[] formatParameters)
        {
            StringBuilder html = new StringBuilder();

            html.AppendFormat("<span class=\"{0}\">", type.ToString().ToLowerInvariant());
            html.AppendFormat(statusMessage, formatParameters);
            html.AppendFormat("</span>");

            FlushScript(string.Format("CS.WriteConsole({0});", HttpUtility.JavaScriptStringEncode(html.ToString(), true)));
        }

		public void WriteLine(string statusMessage, params object[] formatParameters)
        {
            Write(statusMessage + "<br />", formatParameters);
        }

		public void WriteLine(string statusMessage, MessageType type, params object[] formatParameters)
        {
            Write(statusMessage + "<br />", type, formatParameters);
        }

		public void SetProgressStatus(string statusMessage)
        {
            FlushScript(string.Format("CS.SetStatus({0});", HttpUtility.JavaScriptStringEncode(statusMessage, true)));
        }

		public void SetProgress(int percent)
        {
            if (percent < 0 || percent > 100) throw new ArgumentException("Invalid percentage");

            _progress = percent;

            FlushScript(string.Format("CS.SetProgress({0});", _progress));
        }

        public void SetProgress(long itemsProcessed, long totalItems)
        {
            SetProgress((int)Math.Round(((double)itemsProcessed / (double)totalItems) * 100d));
        }

        private void FlushScript(string script)
        {
            _response.Write(string.Format("<script>{0}</script>", script));
			
			var padding = new StringBuilder("<div style=\"display: none;\">");
			var random = new Random();
			for (int i = 0; i < ExtraMessagePaddingLength; i++)
				padding.Append((char)random.Next(33, 126));

			padding.Append("</div>");
			_response.Write(padding);
			
			_response.Flush();
        }

        public int Progress
        {
            get
            {
                return _progress;
            }
        }

		/// <summary>
		/// Extra padding chars added to the end of each message sent to the page. Defeats gzip compression chunking preventing full flushing.
		/// </summary>
		public int ExtraMessagePaddingLength
		{
			get;
			set;
		}
	}
}
