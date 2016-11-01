using System.Collections.Generic;
using System.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace CS422
{
	//used to store the webrequest from the client
	public class WebRequest
	{
		string method = "";
		string uri = "";
		string version = "";
		string type = "text/html";
		Dictionary<string, string> headers = new Dictionary<string, string>();
		Stream body = null; //often concatstream
		NetworkStream network;

		public string Method
		{
			get { return method;}
			set { method = value;}
		}

		public string Type
		{
			get { return type;}
			set { type = value;}
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
						"Content-Length:" + pageHTML.Length + "\r\n\r\n" +
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
						"Content-Type:" + Type + "\r\n" +
				"Content-Length:" + b.Length + "\r\n\r\n" +
									htmlString;

			byte[] bytes = Encoding.ASCII.GetBytes(response);
			//support range header here

			//stream the response

			int size = 8192;

			int written = 0;
			int length = bytes.Length;

			while (length > 0)
			{	if (size > length)
				{
					size = length;
				}

				network.Write(bytes, written, size);
				written += size;
				length -= size;
			}

			return true;
		}

		public bool WriteFileResponse(Stream file)
		{
			string response = null;
			string status = "200 OK";
			int length = (int)file.Length;

			if(headers.ContainsKey("Range"))
			{
				string resultString = Regex.Match(headers["Range"], @"\d+").Value;
				int byteRange = 0;

				//if we have a byte range
				if((byteRange = Int32.Parse(resultString)) != 0)
				{
					if(byteRange < length)
					{
						status = "206";
						length = byteRange;
					}

					else
					{
						status = "416";
					}
				}
			}

			response = "HTTP/" + Version + " " + status + "\r\n" +
						"Content-Type:" + Type + "\r\n" +
				"Content-Length:" + length + "\r\n\r\n";

			byte[] bytes = Encoding.ASCII.GetBytes(response);
			network.Write(bytes, 0, bytes.Length);

			//support range header here

			//stream the response

			byte[] chunk = new byte[1024 * 8];
			int count = 0;
			int read = (int)length;

			while ((count = file.Read(chunk, 0, chunk.Length)) > 0)
			{
				network.Write(chunk, 0, count);

				read -= count;
				if(read < 0)
				{
					return true;
				}
			}


			return true;
		}
	}
}
