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
        private int _progress = 0;

        protected abstract void Process();
        protected abstract string GetTaskName();
        protected virtual string GetPageTitle()
        {
            return "Report Application";
        }

        protected virtual void RenderHead()
        {
            Response.Write(string.Format("<title>{0}</title>", GetPageTitle()));
            Response.Write(string.Format("<link rel=\"stylesheet\" href=\"{0}\" />", Page.ClientScript.GetWebResourceUrl(typeof(WebConsolePage), "Kamsar.WebConsole.Resources.console.css")));
        }

        protected virtual void RenderProgressBar()
        {
            Response.Write("<section class=\"progress\">");
            Response.Write(string.Format("<h2>{0}</h2>", GetTaskName()));
            Response.Write("<div class=\"progressbar\">");
            Response.Write("<span id=\"percentage\">0%</span>");
            Response.Write("<div id=\"bar\"></div>");
            Response.Write("</div>");
            Response.Write("<p id=\"status\"></p>");
            Response.Write("</section>");
        }

        protected virtual void RenderConsole()
        {
            Response.Write("<section id=\"console\">");
            Response.Write("</section>");
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Response.Buffer = false;
            Response.BufferOutput = false;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            Response.Write("<!DOCTYPE html>");

            Response.Write("<html>");
            Response.Write("<head>");
            RenderHead();
            Response.Write("</head>");
            Response.Write("<body>");
            Response.Write("<div class=\"wrapper\">");
            Response.Write(string.Format("<h1>{0}</h1>", GetPageTitle()));

            RenderProgressBar();

            RenderConsole();

            Response.Write("</div>");

            Response.Write(string.Format("<script src=\"{0}\"></script>", Page.ClientScript.GetWebResourceUrl(typeof(WebConsolePage), "Kamsar.WebConsole.Resources.console.js")));
            Response.Flush();

            Process();

            Response.Write("</body>");
            Response.Write("</html>");
            Response.Flush();
        }

        protected void WriteConsole(string statusMessage, params object[] formatParameters)
        {
            WriteConsole(statusMessage, MessageType.Info, formatParameters);
        }

        protected void WriteConsole(string statusMessage, MessageType type, params object[] formatParameters)
        {
            StringBuilder html = new StringBuilder();

            html.AppendFormat("<span class=\"{0}\">", type.ToString().ToLowerInvariant());
            html.AppendFormat(statusMessage, formatParameters);
            html.AppendFormat("</span>");

            FlushScript(string.Format("CS.WriteConsole({0});", HttpUtility.JavaScriptStringEncode(html.ToString(), true)));
        }

        protected void WriteConsoleLine(string statusMessage, params object[] formatParameters)
        {
            WriteConsole(statusMessage + "<br />", formatParameters);
        }

        protected void WriteConsoleLine(string statusMessage, MessageType type, params object[] formatParameters)
        {
            WriteConsole(statusMessage + "<br />", type, formatParameters);
        }

        protected void SetProgressStatus(string statusMessage)
        {
            FlushScript(string.Format("CS.SetStatus({0});", HttpUtility.JavaScriptStringEncode(statusMessage, true)));
        }

        protected void SetProgress(int percent)
        {
            if (percent < 0 || percent > 100) throw new ArgumentException("Invalid percentage");

            _progress = percent;

            FlushScript(string.Format("CS.SetProgress({0});", _progress));
        }

        protected void SetProgress(long itemsProcessed, long totalItems)
        {
            SetProgress((int)Math.Round(((double)itemsProcessed / (double)totalItems) * 100d));
        }

        private void FlushScript(string script)
        {
            Response.Write(string.Format("<script>{0}</script>", script));
            Response.Flush();
        }

        protected int Progress
        {
            get
            {
                return _progress;
            }
        }
    }
}