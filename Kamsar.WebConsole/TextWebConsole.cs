using System;
using System.Text;
using System.Web;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// Variant of the WebConsole that emits the Write/WriteLines done as plain text lines (instead of HTML).
	/// Useful when capturing text logs of a console (eg for non-web-emission contexts)
	/// 
	/// Progress reports are not captured.
	/// 
	/// Requires disposal UNLESS using Render(Action) method
	/// </summary>
	public class TextWebConsole : WebConsole
	{
		private readonly HttpResponseBase _response;

		public TextWebConsole(HttpResponseBase response, bool forceBuffer = true, bool setContentType = true) : base(response, forceBuffer)
		{
			if (setContentType)
			{
				response.ContentType = "text/plain";
			}

			_response = response;
		}

		public override void Render()
		{
			throw new NotImplementedException("Use the overload with the process action to avoid needing to dispose this.");
		}

		public virtual void Render(Action<IProgressStatus> processAction)
		{
			processAction(this);
			Dispose(true);
		}

		public override void Write(string statusMessage, MessageType type, params object[] formatParameters)
		{
			if (type == MessageType.Error) HasErrors = true;
			if (type == MessageType.Warning) HasWarnings = true;

			var line = new StringBuilder();

			line.AppendFormat("{0}: ", type);

			if (formatParameters.Length > 0)
				line.AppendFormat(statusMessage, formatParameters);
			else
				line.Append(statusMessage);

			_response.Write(HttpUtility.HtmlEncode(line.ToString()));
		}

		public override void WriteLine(string statusMessage, MessageType type, params object[] formatParameters)
		{
			Write(statusMessage + "\n", type, formatParameters);
		}

		public override void SetProgress(int percent)
		{
			// do nothing
		}

		public override void WriteScript(string script)
		{
			// do nothing
		}

		public override void SetTransientStatus(string statusMessage, params object[] formatParameters)
		{
			// do nothing
		}

		public virtual bool HasErrors { get; private set; }
		public virtual bool HasWarnings { get; private set; }
	}
}
