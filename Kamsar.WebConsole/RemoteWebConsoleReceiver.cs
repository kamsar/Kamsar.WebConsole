using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// Implements a WebConsole that gets its data feed from a streaming RemoteWebConsole somewhere else
	/// </summary>
	public class RemoteWebConsoleReceiver : WebConsole
	{
		private readonly string _remoteConsoleUrl;
		private readonly ICredentials _credentials;

		public RemoteWebConsoleReceiver(HttpResponseBase response, string remoteConsoleUrl, ICredentials remoteCredentials) : base(response)
		{
			_remoteConsoleUrl = remoteConsoleUrl;
			_credentials = remoteCredentials;

			RemoteSignals = new List<string>();
		}

		public override void Render()
		{
			BeforeProcessRemote?.Invoke(this);

			var client = new WebClient();

			if (_credentials != null) client.Credentials = _credentials;

			using (var readStream = client.OpenRead(_remoteConsoleUrl))
			{
				Debug.Assert(readStream != null, "readStream != null");
				using (var reader = new StreamReader(readStream))
				{
					while (true)
					{
						var line = reader.ReadLine();
						if (line == null) break;

						if (line.StartsWith("SIGNAL::"))
							RemoteSignals.Add(line.Substring(8));
						else
							WriteScript(line);
					}
				}
			}

			AfterProcessRemote?.Invoke(this);
		}

		public IList<string> RemoteSignals { get; }

		public event Action<RemoteWebConsoleReceiver> BeforeProcessRemote;
		public event Action<RemoteWebConsoleReceiver> AfterProcessRemote;
	}
}
