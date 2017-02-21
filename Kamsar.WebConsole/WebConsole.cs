using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// Implements a basic WebConsole in HTML snippets. When using this class you're responsible for wrapping HTML and making sure the console output is emitted at a proper location.
	/// </summary>
	public class WebConsole : IProgressStatus, IDisposable
	{
		private readonly HttpResponseBase _response;
		private bool _resourcesRendered;
		private bool _progressRendered;
		private bool _consoleRendered;
		private readonly Timer _flushTimer;
		private readonly ConcurrentQueue<string> _flushQueue;

		public WebConsole(HttpResponseBase response, bool forceBuffer = true)
		{
			_response = response;

			if (forceBuffer)
			{
				_response.Buffer = false;
				_response.BufferOutput = false;
			}

			MinimumMessageLength = 128;

			_flushTimer = new Timer(FlushQueue, null, Timeout.Infinite, Timeout.Infinite);
			_flushQueue = new ConcurrentQueue<string>();
		}

		private int _progress;

		/// <summary>
		/// Renders the required CSS and JS for the WebConsole. Resources are stored as WebResources.
		/// </summary>
		public virtual void RenderResources()
		{
			var page = new Page();
			_response.Write($"<link rel=\"stylesheet\" href=\"{page.ClientScript.GetWebResourceUrl(typeof(WebConsole), "Kamsar.WebConsole.Resources.console.css")}\" />");
			_response.Write($"<script src=\"{page.ClientScript.GetWebResourceUrl(typeof(WebConsole), "Kamsar.WebConsole.Resources.console.js")}\"></script>");

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
		public virtual void Write(string statusMessage, MessageType type, params object[] formatParameters)
		{
			var html = new StringBuilder();

			html.AppendFormat("<span class=\"{0}\">", type.ToString().ToLowerInvariant());

			if (formatParameters.Length > 0)
				html.AppendFormat(statusMessage, formatParameters);
			else
				html.Append(statusMessage);

			html.Append("</span>");

			WriteScript($"CS.WriteConsole({HttpUtility.JavaScriptStringEncode(html.ToString(), true)});");
		}

		/// <summary>
		/// Writes a message, followed by an end-line, to the WebConsole's console output
		/// </summary>
		public void WriteLine(string statusMessage, params object[] formatParameters)
		{
			WriteLine(statusMessage, MessageType.Info, formatParameters);
		}

		/// <summary>
		/// Writes a message, followed by an end-line, to the WebConsole's console output
		/// </summary>
		public virtual void WriteLine(string statusMessage, MessageType type, params object[] formatParameters)
		{
			Write(statusMessage + "<br />", type, formatParameters);
		}

		/// <summary>
		/// Writes an exception to the WebConsole's console output. Execution continues after the write completes.
		/// </summary>
		public virtual void WriteException(Exception exception)
		{
			var exMessage = new StringBuilder();
			exMessage.AppendFormat("ERROR: {0} ({1})", exception.Message, exception.GetType().FullName);
			exMessage.Append("<div class=\"stacktrace\">");

			exMessage.Append(exception.StackTrace?.Trim().Replace("\n", "<br />") ?? "No stack trace available.");

			exMessage.Append("</div>");

			WriteInnerException(exception.InnerException, exMessage);

			WriteLine(exMessage.ToString(), MessageType.Error);

			if (Debugger.IsAttached) Debugger.Break();
		}

		private static void WriteInnerException(Exception innerException, StringBuilder exMessage)
		{
			if (innerException == null) return;

			exMessage.Append("<div class=\"innerexception\">");
			exMessage.AppendFormat("{0} ({1})", innerException.Message, innerException.GetType().FullName);
			exMessage.Append("<div class=\"stacktrace\">");

			exMessage.Append(innerException.StackTrace?.Trim().Replace("\n", "<br />") ?? "No stack trace available.");

			WriteInnerException(innerException.InnerException, exMessage);

			exMessage.Append("</div>");
		}

		/// <summary>
		/// Writes a progress status message to the status line beneath the progress bar.
		/// </summary>
		public virtual void SetTransientStatus(string statusMessage, params object[] formatParameters)
		{
			string status = statusMessage;
			if (formatParameters.Length > 0) status = string.Format(statusMessage, formatParameters);

			WriteScript($"CS.SetStatus({HttpUtility.JavaScriptStringEncode(status, true)});");
		}

		/// <summary>
		/// Sets the percentage complete display
		/// </summary>
		public virtual void SetProgress(int percent)
		{
			if (percent < 0 || percent > 100) throw new ArgumentException("Invalid percentage");

			if (percent == _progress) return;

			_progress = percent;

			WriteScript($"CS.SetProgress({_progress});");
		}

		/// <summary>
		/// Sets the percentage complete display, given a proportion of completeness
		/// </summary>B
		public void SetProgress(long itemsProcessed, long totalItems)
		{
			SetProgress((int)Math.Round((itemsProcessed / (double)totalItems) * 100d));
		}


		/// <summary>
		/// Writes and executes a JavaScript statement on the console page. You don't need script tags, only JS content.
		/// </summary>
		public virtual void WriteScript(string script)
		{
			if (_flushQueue.Count == 0)
			{
				_flushTimer.Change(500, Timeout.Infinite);
			}

			_flushQueue.Enqueue(script);
			
		}

		protected void FlushQueue(object state)
		{
			var scripts = new StringBuilder();
			string current;
			while (_flushQueue.TryDequeue(out current))
			{
				scripts.AppendLine(current);
			}

			_response.Write($"<script>{scripts} CS.BatchComplete();</script>");

			if (scripts.Length < MinimumMessageLength)
			{
				var padding = new StringBuilder("<div style=\"display: none;\">");
				var random = new Random();
				for (int i = scripts.Length; i < MinimumMessageLength; i++)
				{
					padding.Append((char) random.Next(33, 126));
				}

				padding.Append("</div>");

				_response.Write(padding);
			}

			try
			{
				_response.Flush();
			}
			catch (HttpException)
			{
				// Client disconnected
			}
		}

		/// <summary>
		/// Gets the current completion percentage
		/// </summary>
		public int Progress => _progress;

		/// <summary>
		/// Extra padding chars added to the end of each message sent to the page. Defeats gzip compression chunking preventing full flushing.
		/// </summary>
		public int MinimumMessageLength { get; set; }

		void IProgressStatus.Report(int percent)
		{
			SetProgress(percent);
		}

		void IProgressStatus.ReportException(Exception exception)
		{
			WriteException(exception);
		}

		void IProgressStatus.ReportStatus(string statusMessage, params object[] formatParameters)
		{
			((IProgressStatus)this).ReportStatus(statusMessage, MessageType.Info, formatParameters);
		}

		void IProgressStatus.ReportStatus(string statusMessage, MessageType type, params object[] formatParameters)
		{
			WriteLine(statusMessage, type, formatParameters);
		}

		void IProgressStatus.ReportTransientStatus(string statusMessage, params object[] formatParameters)
		{
			SetTransientStatus(statusMessage, formatParameters);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				FlushQueue(null);
				_flushTimer?.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}

