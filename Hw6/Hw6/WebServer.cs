using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace CS422
{
	public class WebServer
	{
		static IPAddress addr = IPAddress.Parse("127.0.0.1");
		static Thread[] pool;
		static List<WebService> services = new List<WebService>();
		static int SIZE = 64;

		public static void Start(Int32 port, Int32 threads)
		{
			TcpListener server = null;

			if(threads <= 0)
			{
				threads = 64;
			}

			//set the amount of threads to the pool
			pool = new Thread[threads];

			server = new TcpListener(addr, port);

			//add the litener method to the thread
			pool[0] = new Thread(listen);
			pool[0].Start(server);

			while(pool[0].IsAlive) {}
		}

		//thread function for listening
		private static void listen(object s)
		{
			if (s is TcpListener)
			{
				TcpListener server = s as TcpListener;

				server.Start();
				MemoryStream body = new MemoryStream();

				while (true)
				{
					// Enter the listening loop.
					Console.Write("Waiting for a connection... ");

					// Perform a blocking call to accept requests.
					// You could also user server.AcceptSocket() here.
					TcpClient client = server.AcceptTcpClient();

					lock(client)
					{
						Console.WriteLine("Connected!");

						Thread t = ThreadWorks();

						lock(t)
						{
							if (t != null)
							{
								t.Start(client);
								while (t.IsAlive) { }
							}
						}
					}
				}
			}
		}

		//only need to support the get request
		private static WebRequest BuildRequest(TcpClient client)
		{
			bool run = true;
			byte[] bytes = new byte[SIZE];
			StringBuilder data = new StringBuilder();

			StringBuilder temp = new StringBuilder();

			WebRequest req = new WebRequest();

			NetworkStream stream = client.GetStream();

			int phase = 0;
			int crlfCount = 0;

			req.Network = stream;
			MemoryStream body = new MemoryStream(64);
			body.SetLength(64);
			req.Body = new ConcatStream(body, stream);

			while(run)
			{
				//read the data and store it to a string
				int i = stream.Read(bytes, 0, SIZE);
				int counter = 0;
				string s = System.Text.Encoding.ASCII.GetString(bytes);
				data.Append(s);

				if(s == null)
				{
					break;
				}

				if(temp != null)
				{
					s = temp.ToString() + s;
					temp = new StringBuilder();
				}

				//phase 0: get method
				if(phase == 0)
				{
					//for every character in the string
					while(counter < s.Length)
					{
						//if we run into a space we are now at the next segment
						if(s[counter] == ' ')
						{
							req.Method = s.Substring(0, counter);
							break;
						}

						counter++;
					}

					//move forward, we should now have the '/'
					counter++;
					if(s[counter] != '/')
					{
						return null;
					}

					phase++;
				}

				//phase 1: get URI 
				if (phase == 1)
				{
					while(counter < s.Length)
					{
						if (s[counter] == ' ')
						{
							phase++;
							counter++;
							req.URI = temp.ToString();
							temp = new StringBuilder();
							break;
						}

						else { temp.Append(s[counter]);}

						counter++;
					}
				}

				//phase 2: check HTTP type
				if (phase == 2)
				{
					while(counter < s.Length)
					{
						//at the end of the http type
						if (temp.ToString().Length > 1)
						{
							if (temp.ToString().Substring(temp.ToString().Length - 2, 2) == "\r\n")
							{
								string http = temp.ToString().Substring(0, temp.ToString().Length - 2);
								//since we are only supporting http/1.1
								//we can hardcode the check
								if (http == "HTTP/1.1")
								{
									phase++;
									req.Version = http;
									temp = new StringBuilder();
									break;
								}

								else
								{
									return null;
								}
							}
						}


						temp.Append(s[counter]);

						counter++;
					}
				}

				//phase 3: get headers
				if (phase == 3)
				{
					while (counter < s.Length)
					{
						//at the end of the http type
						if (temp.ToString().Length > 1)
						{
							if (temp.ToString().Substring(temp.ToString().Length - 2, 2) == "\r\n")
							{
								//if we have a double carriage return only, we
								//have ht the double carriage return
								if (temp.Length == 2 && crlfCount > 0)
								{
									phase++;
									temp = new StringBuilder();
									break;
								}

								else
								{
									string header = temp.ToString().Substring(0, temp.ToString().Length - 2);
									//since we are only supporting http/1.1
									//we can hardcode the check

									string key = header.Split(": ".ToCharArray(), 2)[0];
									string val = header.Split(": ".ToCharArray(), 2)[1];

									if (key != null && val != null)
									{
										//create a sizable length for the stream
										if(header == "Content-Length")
										{
											int size = 0;

											if(Int32.TryParse(val, out size))
											{
												req.Body.SetLength((long)size);
											}
										}

										req.Headers.Add(key, val.Substring(1));
									}

									temp = new StringBuilder();
								}

								crlfCount++;
							}
						}

						if (counter < s.Length)
						{
							temp.Append(s[counter]);

							counter++;
						}
					}
				}

				//reading the body
				if(phase == 4)
				{
					//write to body
					while(counter < s.Length)
					{
						byte b = Convert.ToByte(s[counter]);
						byte[] arr = { b }; 
						req.Body.Write(arr, 0, arr.Length);
						counter++;
					}
				}

				//if we have read the end of the stream
				if(i < SIZE)
				{
					run = false;
				}
			}

			//return the object if we have successfully gone through each step
			if (phase >= 3) 
			{
				return req; 
			}

			else { return null;}
		}

		//thread works function
		//if the request-handling function 
		//for a thread from the thread pool
		private static Thread ThreadWorks()
		{
			lock(pool)
			{
				//go through each of the available threads to see
				//which ones are still running
				for (int i = 1; i < pool.Length; i++)
				{
					if (pool[i] != null) 
					{
						if (!pool[i].IsAlive)
						{
							pool[i] = new Thread(new ParameterizedThreadStart(threadRun));
							return pool[i];
						}
					}

					else
					{
						pool[i] = new Thread(new ParameterizedThreadStart(threadRun));
						return pool[i];
					}
				}
			}

			return null;
		}

		//thread for running works on the client that we have gathered
		private static void threadRun(object client)
		{
			if(client is TcpClient)
			{
				TcpClient tcp = client as TcpClient;

				CS422.WebRequest req = BuildRequest(tcp);

				if (req != null)
				{
					foreach (WebService w in services)
					{
						w.Handler(req);
					}
				}

				else
				{
					string message = "Valid HTTP request was not found.";
					byte[] b = Encoding.ASCII.GetBytes(message);
					Stream s = tcp.GetStream();
					s.Write(b, 0, b.Length);
				}

				tcp.GetStream().Close();
				tcp.Close();
			}
		}

		//adds a webservice to the client
		//make sure it is thread safe
		//to find out if a webservice can handle a requested
		//we need to check:
		//request-target URI starts with the string
		//specified by the WebService object's Service URI parameter
		public static void AddService(WebService service)
		{
			//lock our services list to stop other threads from
			//messing with it while we are adding our service
			lock(services)
			{
				//if the service requested to be added
				//does not already exists
				if(!services.Contains(service))
				{
					services.Add(service);
				}
			}
		}

		//implement the blocking stop function
		//lets all threads finish their tasks then
		//terminate all threads in the pool
		public static void Stop()
		{
			//lock the pool so it cannot be 
			//accessed by any other program
			lock(pool)
			{
				//abort the listening thread since we will
				//not be accepting any more clients
				pool[0].Abort();

				//iterate through every
				for (int i = 1; i < pool.Length; i++)
				{
					//block while the pool is alive
					while (pool[i].IsAlive) { }
					pool[i] = null;
				}

				//exit the program
				Environment.Exit(0);
			}
		}
	}
}

