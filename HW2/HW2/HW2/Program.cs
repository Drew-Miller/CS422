using System;
using CS422;
using System.Threading;
using System.Collections.Generic;

namespace HW2
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			/*
			byte[] bArray = new byte[args.Length];

			int i = 0;
			bool first = false;
			int sleep = 0;
			foreach(string s in args)
			{
				int x = 0;
				if(Int32.TryParse(s, out x))
				{
					if (!first)
					{
						sleep = x * 1000;
						Console.WriteLine("Sleep: " + sleep);
						first = true;
					}

					else
					{
						bArray[i] = (byte)x;
						i++;
					}
				}
			}

			ThreadPoolSleepSorter tpss = new ThreadPoolSleepSorter(Console.Out, 128);
			tpss.Sort(bArray);

			System.Threading.Thread.Sleep(sleep);
			tpss.Waste();*/

			int num = 42; lock(num) {};
			string s = "Hello"; lock(s) {};
			List<int> myList = new List<int>(); lock (myList) { }
			string s = string.Empty; lock (s) { }


		}
	}
}
