using System;
using System.Web;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// A web console that does nothing. Useful for passing in cases where a console is normally used, but you don't care about status outputs.
	/// </summary>
	public class NullWebConsole : WebConsole
	{
		public NullWebConsole() : base(HttpContext.Current.Response)
		{
				
		}

		public override void Render()
		{
			
		}

		public override void SetProgress(int percent)
		{
			
		}

		public override void SetProgressStatus(string statusMessage, params object[] formatParameters)
		{
		}

		public override void RenderConsole()
		{
			
		}

		public override void RenderProgressBar()
		{
			
		}

		public override void RenderResources()
		{
			
		}

		public override void Write(string statusMessage, MessageType type, params object[] formatParameters)
		{
			
		}

		public override void WriteException(Exception exception)
		{
			
		}

		public override void WriteLine(string statusMessage, MessageType type, params object[] formatParameters)
		{
			
		}

		public override void WriteScript(string script)
		{
			
		}
	}
}
