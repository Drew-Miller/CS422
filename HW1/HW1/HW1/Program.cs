using System;
using System.IO;
using CS422;

namespace HW1
{
	class MainClass
	{
		//INS consts
		const int SSIZE = 257;
		const int MOD = 256;
		const int BSIZE = 10;
		const int COUNT = 10;
		const int OFFSET = 2;

		public static void Main(string[] args)
		{
			Console.WriteLine("INS Testing:");
			byte[] buf1 = new byte[257];
			byte[] buf2 = new byte[BSIZE];
			IndexedNumsStream ins = new IndexedNumsStream(SSIZE);
			ins.Read(buf1, 0, 257);
			ins.Read(buf2, 0, COUNT);

			for (int i = 0; i < 257; i++)
			{
				Console.WriteLine(buf1[i]);
			}

			for (int i = 0; i < BSIZE; i++)
			{
				Console.WriteLine(buf2[i]);
			}
		}
	}
}
