using System.Text;
using System.Web;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// Variant of the WebConsole that saves the lines to a StringBuilder for programmatic capture
	/// 
	/// Progress reports are not captured.
	/// </summary>
	public class StringWebConsole : WebConsole
	{
		StringBuilder _output = new StringBuilder();
		public StringWebConsole() : base(HttpContext.Current.Response)
		{
		}

		public override void Write(string statusMessage, MessageType type, params object[] formatParameters)
		{
			var line = new StringBuilder();

			line.AppendFormat("{0}: ", type);

			if (formatParameters.Length > 0)
				line.AppendFormat(statusMessage, formatParameters);
			else
				line.Append(statusMessage);

			_output.Append(HttpUtility.HtmlEncode(line.ToString()));
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

		public string Output { get { return _output.ToString(); } }
	}
}
