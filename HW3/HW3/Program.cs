using System;

namespace CS422
{
	class MainClass
	{
		const Int32 port = 15000;

		const string response = 
			"HTTP/1.1 200 OK\r\n" +
 			"Content-Type: text/html\r\n" +
			"\r\n\r\n" +
			"<html>ID Number: {0}<br>" +
			"DateTime.Now: {1}<br>" +
			"Requested URL: {2}</html>";

		public static void Main(string[] args)
		{
			if(args.Length == 0)
			{
				Console.WriteLine(WebServer.Start(port, response));
			}
			else if (args[0] == "1")
			{
				WebServer.Start(port, response);
			}

			else
			{
				Client.Connect("127.0.0.1", port,
								"GET /oogabooga HTTP/1.1\r\n \r\n\r\n"
							  );
			}
		}
	}
}
