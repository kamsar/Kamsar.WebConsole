using System.Web;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// A WebConsole "API" that sends the raw console command scripts only. The RemoteWebConsoleReceiver is a type of console specialized to receive these remote scripts.
	/// You can use this to "syndicate" a console's output from a remote service into another application.
	/// </summary>
	public class RemoteWebConsole : WebConsole
	{
		readonly HttpResponseBase _response;

		public RemoteWebConsole(HttpResponse response) : base(response)
		{
			_response = new HttpResponseWrapper(response);
		}

		public RemoteWebConsole(HttpResponseBase response)
			: base(response)
		{
			_response = response;
		}

		public override void WriteScript(string script)
		{
			// we write the raw script out with an endline; we pick this up with ReadLine() on the receiver
			// note this may be unreliable if gzipping the response because it does not have any padding.
			_response.Write(script + "\n");
			_response.Flush();
		}

		/// <summary>
		/// Sends a "signal" to the console receiver. Signals are stored in a collection and can be reviewed.
		/// For example a signal to perform a redirect might be used for when a process succeeds or fails.
		/// </summary>
		public void WriteRemoteSignal(string signalContent)
		{
			WriteScript("SIGNAL::" + signalContent);
		}
	}
}
