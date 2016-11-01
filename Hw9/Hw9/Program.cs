using System;

namespace CS422
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			StandardFileSystem sfs = StandardFileSystem.Create("/Users");
			WebServer.AddService(new FileWebService(sfs));
			WebServer.Start(13000, 32);
			WebServer.Stop();
		}
	}
}
