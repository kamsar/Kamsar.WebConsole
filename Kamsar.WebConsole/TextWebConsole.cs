using System.Text;
using System.Web;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// Variant of the WebConsole that emits the Write/WriteLines done as text lines.
	/// Useful when capturing text logs of a console (eg for non-web-emission contexts)
	/// 
	/// Progress reports are not captured.
	/// </summary>
	public class TextWebConsole : WebConsole
	{
		private readonly HttpResponseBase _response;

		public TextWebConsole(HttpResponseBase response) : base(response)
		{
			_response = response;
		}

		public TextWebConsole(HttpResponse response)
			: base(new HttpResponseWrapper(response))
		{
			_response = new HttpResponseWrapper(response);
		}

		public override void Write(string statusMessage, MessageType type, params object[] formatParameters)
		{
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

		public override void SetProgressStatus(string statusMessage, params object[] formatParameters)
		{
			// do nothing
		}
	}
}
