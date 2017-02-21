using System;
using System.Web;

namespace Kamsar.WebConsole
{
	/// <summary>
	/// Implements a WebConsole that has a basic HTML5 page wrapper on it.
	/// Note that this class does not require disposal if you're using the Render(Action) method.
	/// </summary>
	public class Html5WebConsole : WebConsole
	{
		private readonly HttpResponseBase _response;

		public Html5WebConsole(HttpResponseBase response) : base(response)
		{
			_response = response;
			Title = string.Empty;
		}

		public Html5WebConsole(HttpResponse response)
			: this(new HttpResponseWrapper(response))
		{
		}

		public string Title { get; set; }

		/// <summary>
		/// Renders content into the head tag
		/// </summary>
		protected virtual void RenderHead()
		{
			if (!string.IsNullOrEmpty(Title))
			{
				_response.Write($"<title>{Title}</title>");
			}

			RenderResources();
		}

		/// <summary>
		/// Renders heading content into the page (e.g. a h1 of the title, etc)
		/// </summary>
		protected virtual void RenderPageHead()
		{
			if (!string.IsNullOrEmpty(Title))
			{
				_response.Write($"<h1>{HttpUtility.HtmlEncode(Title)}</h1>");
			}
		}

		public override void Render()
		{
			throw new NotImplementedException("Use the overload with the process action to place your console output at a proper location in the markup.");
		}

		public virtual void Render(Action<IProgressStatus> processAction)
		{
			_response.Write("<!DOCTYPE html>");

			_response.Write("<html>");
			_response.Write("<head>");

			RenderHead();

			_response.Write("</head>");
			_response.Write("<body>");
			_response.Write("<div class=\"wrapper\">");

			RenderPageHead();

			base.Render();

			_response.Write("</div>");
			_response.Flush();

			processAction(this);

			Dispose(true);

			_response.Write("</body>");
			_response.Write("</html>");
			_response.Flush();
		}
	}
}
