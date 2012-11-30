using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// Implements a basic WebConsole in HTML snippets. When using this class you're responsible for wrapping HTML and making sure the console output is emitted at a proper location.
	/// </summary>
	public class WebConsole
	{
		private HttpResponseBase _response;
		bool _resourcesRendered = false;
		bool _progressRendered = false;
		bool _consoleRendered = false;

		public WebConsole(HttpResponseBase response)
		{
			_response = response;
			_response.Buffer = false;
			_response.BufferOutput = false;

			ExtraMessagePaddingLength = 128;
		}

		public WebConsole(HttpResponse response)
			: this(new HttpResponseWrapper(response))
		{
		}

        private int _progress = 0;

		/// <summary>
		/// Renders the required CSS and JS for the WebConsole. Resources are stored as WebResources.
		/// </summary>
		public void RenderResources()
		{
			var page = new Page();
			_response.Write(string.Format("<link rel=\"stylesheet\" href=\"{0}\" />", page.ClientScript.GetWebResourceUrl(typeof(WebConsolePage), "Kamsar.WebConsole.Resources.console.css")));
			_response.Write(string.Format("<script src=\"{0}\"></script>", page.ClientScript.GetWebResourceUrl(typeof(WebConsolePage), "Kamsar.WebConsole.Resources.console.js")));

			_resourcesRendered = true;
		}

		/// <summary>
		/// Renders the progress bar portion of the WebConsole.
		/// </summary>
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

		/// <summary>
		/// Renders the output console portion of the WebConsole.
		/// </summary>
        public virtual void RenderConsole()
        {
			_response.Write("<section id=\"console\">");
			_response.Write("</section>");

			_consoleRendered = true;
        }

		/// <summary>
		/// Renders all WebConsole components. If some components have already been rendered individually, they will not be rendered again.
		/// </summary>
		public virtual void Render()
        {
			// these rendered flags allow you to decide where you want these components in source by invoking their renderings prior to the main render
			if (!_resourcesRendered) RenderResources();
            if (!_progressRendered) RenderProgressBar();
            if (!_consoleRendered) RenderConsole();

			_response.Flush();
        }

		/// <summary>
		/// Writes a message to the WebConsole's console output
		/// </summary>
		public void Write(string statusMessage, params object[] formatParameters)
        {
            Write(statusMessage, MessageType.Info, formatParameters);
        }

		/// <summary>
		/// Writes a message to the WebConsole's console output
		/// </summary>
		public void Write(string statusMessage, MessageType type, params object[] formatParameters)
        {
            StringBuilder html = new StringBuilder();

            html.AppendFormat("<span class=\"{0}\">", type.ToString().ToLowerInvariant());
            html.AppendFormat(statusMessage, formatParameters);
            html.AppendFormat("</span>");

            WriteScript(string.Format("CS.WriteConsole({0});", HttpUtility.JavaScriptStringEncode(html.ToString(), true)));
        }

		/// <summary>
		/// Writes a message, followed by an end-line, to the WebConsole's console output
		/// </summary>
		public void WriteLine(string statusMessage, params object[] formatParameters)
        {
            Write(statusMessage + "<br />", formatParameters);
        }

		/// <summary>
		/// Writes a message, followed by an end-line, to the WebConsole's console output
		/// </summary>
		public void WriteLine(string statusMessage, MessageType type, params object[] formatParameters)
        {
            Write(statusMessage + "<br />", type, formatParameters);
        }

		/// <summary>
		/// Writes an exception to the WebConsole's console output. Execution continues after the write completes.
		/// </summary>
		public void WriteException(Exception exception)
		{
			StringBuilder exMessage = new StringBuilder();
			exMessage.AppendFormat("ERROR: {0} ({1})", exception.Message, exception.GetType().FullName);
			exMessage.Append("<div class=\"stacktrace\">");

			if (exception.StackTrace != null)
				exMessage.Append(exception.StackTrace.Trim().Replace("\n", "<br />"));
			else
				exMessage.Append("No stack trace available.");

			exMessage.Append("</div>");

			WriteInnerException(exception.InnerException, exMessage);

			WriteLine(exMessage.ToString(), MessageType.Error);
		}

		private static void WriteInnerException(Exception innerException, StringBuilder exMessage)
		{
			if (innerException == null) return;

			exMessage.Append("<div class=\"innerexception\">");
			exMessage.AppendFormat("{0} ({1})", innerException.Message, innerException.GetType().FullName);
			exMessage.Append("<div class=\"stacktrace\">");

			if (innerException.StackTrace != null)
				exMessage.Append(innerException.StackTrace.Trim().Replace("\n", "<br />"));
			else
				exMessage.Append("No stack trace available.");

			WriteInnerException(innerException.InnerException, exMessage);

			exMessage.Append("</div>");
		}

		/// <summary>
		/// Writes a progress status message to the status line beneath the progress bar.
		/// </summary>
		public void SetProgressStatus(string statusMessage, params object[] formatParameters)
        {
            WriteScript(string.Format("CS.SetStatus({0});", HttpUtility.JavaScriptStringEncode(string.Format(statusMessage, formatParameters), true)));
        }

		/// <summary>
		/// Sets the percentage complete display
		/// </summary>
		public void SetProgress(int percent)
        {
            if (percent < 0 || percent > 100) throw new ArgumentException("Invalid percentage");

			if (percent == _progress) return;

            _progress = percent;

            WriteScript(string.Format("CS.SetProgress({0});", _progress));
        }

		/// <summary>
		/// Sets the percentage complete display, given a proportion of completeness
		/// </summary>
        public void SetProgress(long itemsProcessed, long totalItems)
        {
            SetProgress((int)Math.Round(((double)itemsProcessed / (double)totalItems) * 100d));
        }

		/// <summary>
		/// Sets the progress of the whole based on the progress within a sub-task of the main progress (e.g. 0-100% of a task within the global range of 0-20%)
		/// </summary>
		/// <param name="taskNumber">The index of the current sub-task</param>
		/// <param name="totalTasks">The total number of sub-tasks</param>
		/// <param name="taskPercent">The percentage complete of the sub-task (0-100)</param>
		public void SetTaskProgress(int taskNumber, int totalTasks, int taskPercent)
		{
			if (taskNumber < 1) throw new ArgumentException("taskNumber must be 1 or more");
			if (totalTasks < 1) throw new ArgumentException("totalTasks must be 1 or more");

			int start = (int)Math.Round(((taskNumber-1) / (double)totalTasks) * 100d);
			int end = start + (int)Math.Round((1d / (double)totalTasks) * 100d);

			SetRangeTaskProgress(Math.Max(start, 0), Math.Min(end, 100), taskPercent);
		}

		/// <summary>
		/// Sets the progress of the whole based on the progress within a percentage range of the main progress (e.g. 0-100% of a task within the global range of 0-20%)
		/// </summary>
		/// <param name="startPercentage">The percentage the task began at</param>
		/// <param name="endPercentage">The percentage the task ends at</param>
		/// <param name="taskPercent">The percentage complete of the sub-task (0-100)</param>
		public void SetRangeTaskProgress(int startPercentage, int endPercentage, int taskPercent)
		{
			int range = endPercentage - startPercentage;

			if (range <= 0) throw new ArgumentException("endPercentage must be greater than startPercentage");

			int offset = (int)Math.Round(range * (taskPercent / 100d));

			SetProgress(Math.Min(startPercentage + offset, 100));
		}

		/// <summary>
		/// Writes and executes a JavaScript statement on the console page. You don't need script tags, only JS content.
		/// </summary>
        public void WriteScript(string script)
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

		/// <summary>
		/// Gets the current completion percentage
		/// </summary>
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
