using System.Collections.Generic;
using System.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CS422
{
	//used to store the webrequest from the client
	public class WebRequest
	{
		string method = "";
		string uri = "";
		string version = "";
		Dictionary<string, string> headers = new Dictionary<string, string>();
		Stream body = null; //often concatstream
		NetworkStream network;

		public string Method
		{
			get { return method;}
			set { method = value;}
		}

		public string URI
		{
			get { return uri;}
			set { uri = value;}
		}

		public NetworkStream Network
		{
			set { network = value;}
		}

		public string Version
		{
			get { return version;}
			set { version = value;}
		}

		public Dictionary<string, string> Headers
		{
			get { return headers;}
			set { headers = value;	}
		}

		public Stream Body
		{
			get { return body;}
			set { body = value;	}
		}

		//writes 404 status code and the specified HTML string as the body
		//of the response
		public void WriteNotFoundResponse(string pageHTML)
		{
			string response = null;
			response = "HTTP/" + Version + " 404 NotFound\r\n" +
						"Content-Type:text/html\r\n" +
						"Content-Length:" + headers["Content-Length"] + "\r\n\r\n" +
						pageHTML;

			byte[] bytes = Encoding.ASCII.GetBytes(response);
			network.Write(bytes, 0, bytes.Length);
		}

		//writes a 200 status code and the specified HTMl string as the
		//body of the response
		public bool WriteHtmlResponse(string htmlString)
		{
			string response = null;

			byte[] b = Encoding.ASCII.GetBytes(htmlString);

			response = "HTTP/" + Version + " 200 OK\r\n" +
						"Content-Type:text/html\r\n" +
						"Content-Length:" + b.Length + "\r\n\r\n" +
						htmlString;

			byte[] bytes = Encoding.ASCII.GetBytes(response);
			network.Write(bytes, 0, bytes.Length);

			return true;
		}
	}
}
