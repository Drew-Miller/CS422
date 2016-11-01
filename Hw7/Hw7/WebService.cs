using System;
namespace CS422
{
	public abstract class WebService
	{
		//app that runs on top of the server core
		public abstract void Handler(WebRequest req);

		//returns the service URI.
		//in the form /MyServiceName.whatever
		public abstract string ServiceURI
		{
			get;
		}
	}
}
