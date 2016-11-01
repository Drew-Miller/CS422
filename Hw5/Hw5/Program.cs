using System;
using System.IO;
using System.Text;

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
			
		}
	}
}
