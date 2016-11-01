using System;

namespace CS422
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			WebServer.AddService(new DemoService());
			WebServer.Start(13000, 32);
			WebServer.Stop();
		}
	}
}
