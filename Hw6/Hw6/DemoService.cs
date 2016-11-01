using System;

namespace CS422
{

	//writes the get request html template
	public class DemoService : WebService
	{
		private const string c_template =
			"<html>This is the response to the request:<br>" +
 			"Method: {0}<br>Request-Target/URI: {1}<br>" +
		 	"Request body size, in bytes: {2}<br><br>" +
 			"Student ID: {3}</html>";

		public override string ServiceURI
		{
			get
			{
				return "/";
			}
		}

		//have this write back to the request
		public override void Handler(WebRequest req)
		{
			if (!req.Headers.ContainsKey("Content-Length"))
			{
				int length = 0;

				if (req.Body != null)
				{
					length = (int)req.Body.Length;
				}

				req.Headers.Add("Content-Length", length.ToString());
			}

			//call the service on the request
			if (req.URI.StartsWith(ServiceURI, StringComparison.Ordinal))
			{
				req.WriteHtmlResponse(string.Format(c_template, req.Method,
									  req.URI, req.Headers["Content-Length"],
													   "11382134"));
			}
			else
			{
				req.WriteNotFoundResponse(string.Format(c_template, req.Method,
									  req.URI, req.Headers["Content-Length"],
													   "11382134"));
			}
		}
	}
}
