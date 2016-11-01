using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CS422
{
	public class program
	{
		static int Main(string[] args)
		{
			StandardFileSystem sfs2 = StandardFileSystem.Create("/Users/drewm");

			Dir422 dir1 = sfs2.GetRoot().GetDir("Desktop");

			//Assert.NotNull(dir1.CreateFile("hello.txt"));
			//Assert.NotNull(dir1.CreateFile("Test.txt"));

			dir1.CreateFile("A");
			string datastring = "new data";
			byte[] info = new UTF8Encoding(true).GetBytes(datastring);

			File422 a = dir1.GetFile("A");
			if (a is StdFSFile)
			{
				a = a as StdFSFile;

				FileStream fs = (FileStream)((StdFSFile)a).OpenReadWrite();
				fs.Write(info, 0, info.Length);
				fs.Close();
			}

			return 0;
		}
	}
}
