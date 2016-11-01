using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace CS422
{
	public class WebServer
	{
		const int SIZE = 256;
		const string ID = "11382134";
		static IPAddress addr = IPAddress.Parse("127.0.0.1");

		//dictionary to match http elements
		private const String HTTP = "GET / HTTP/1.1\r\n ";
		private const string END = "\r\n\r\n";

		private static string _tResp;

		public WebServer()
		{
		}

		public static bool Start(Int32 port, string templateResponse)
		{
			TcpListener server = null;
			_tResp = templateResponse;  

			// TcpListener server = new TcpListener(port);
			server = new TcpListener(addr, port);

			// Start listening for client requests.
			server.Start();

			// Buffer for reading data
			Byte[] bytes = new Byte[256];
			String data = null;

			// Enter the listening loop.
			Console.Write("Waiting for a connection... ");

			// Perform a blocking call to accept requests.
			// You could also user server.AcceptSocket() here.
			TcpClient client = server.AcceptTcpClient();
			Console.WriteLine("Connected!");

			data = null;

			// Get a stream object for reading and writing
			NetworkStream stream = client.GetStream();

			//index in string
			int place = 0;
			int dCRLF = 0;
			bool run = true;
			StringBuilder sb = new StringBuilder();

			while (run)
			{
				int i = stream.Read(bytes, 0, SIZE);
				//get data from stream
				data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
				sb.Append(data);

				//test data here for correct value
				//parse through each letter in the input to
				//determine if correct syntax
				for (int j = 0; j < data.Length; j++)
				{
					char tempD = data[j];
					char tempH = HTTP[place];
					char tempE = END[dCRLF];

					//if we are at a space
					if(HTTP[place] == ' ' && place != (HTTP.Length - 1))
					{
						if(place == 3)
						{
							if(data[j] != ' ')
							{
								client.Close();
								stream.Dispose();
								return false;
							}
							else
							{
								place++;
							}
						}
						//if we are at the URI of the request
						else if(place == 5)
						{
							//if we reach the end of the URI
							if(data[j] == ' ')
							{
								//we move forward in place
								place++;
							}

							//else, we do nothing. keep reading the
							//URI of undetermined length
						}
					}

					//if we are at the body of the request
					else if (place == (HTTP.Length - 1))
					{
						//if we hit the double carriage return
						if (data[j] == '\r' && dCRLF == 0)
						{
							dCRLF++;
						}

						//if we might have hit a double Carriage Return Line Feed,
						//we test to see if we make it to the end
						else if(dCRLF > 0)
						{
							//if we hit the end of the END statement,
							//then we have done it boys
							if (dCRLF == (END.Length - 1))
							{
								writeS(stream, sb.ToString());
								client.Close();
								stream.Dispose();
								return true;
							}

							//if we have not a match, we break
							if(data[j] != END[dCRLF])
							{
								dCRLF = 0;
							}

							//if we do have match, we increment
							else
							{
								dCRLF++;
							}
						}

						//else, we do nothing.
						//keep reading the body section 
						//of an undetermined length
					}

					//if we are not in a looping section
					//AKA the body or URI, the strings
					//must match AKA the head, HTTP, or double crlf
					else
					{
						//if the strings do not match in an area
						if(data[j] != HTTP[place])
						{
							//DO NOT WRITE!!
							client.Close();
							stream.Dispose();
							return false;
						}

						else
						{
							place++;
						}
					}
				}

				//we break out here if we did not get to the end of the HTTP request or
				//receive an invalid one. Two things happened.
				//one: it is a valid request, but we did not receieve all of it yet
				//or two: it does not have a valid end to the request and therefore
				//isn't valid.

				//if we reached the end of stream and did
				//not match an HTTP request
				if(i < SIZE)
				{
					//DO NOT WRITE!
					client.Close();
					stream.Dispose();
					return false;
				}
			}

			//should never hit here, but if somehow it passed the tests it should
			//be valid
			writeS(stream, sb.ToString());
			// Shutdown and end connection
			client.Close();
			stream.Dispose();
			return true;
		}

		public static void writeS(NetworkStream s, String d)
		{
			//gets url from string
			string url = extractURL(d);

			//puts the items in the respnose template
			string resp = string.Format(_tResp, ID, DateTime.Now, url);

			//converts string to msg
			byte[] msg = System.Text.Encoding.ASCII.GetBytes(resp);
			//parse the message and pass back the formatted string
			s.Write(msg, 0, msg.Length);
		}

		//takes the URL out of HTTP code
		public static string extractURL(string s)
		{
			StringBuilder url = new StringBuilder();

			int i = 5;
			while(s[i] != ' ')
			{
				char c = s[i];
				url.Append(s[i]);
				i++;
			}

			return url.ToString();
		}
	}
}

