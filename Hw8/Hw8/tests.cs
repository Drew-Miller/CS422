using System;
using NUnit.Framework;
using System.Text;
using System.IO;

namespace CS422
{
	[TestFixture]
	public class tests
	{
		private static string current = AppDomain.CurrentDomain.BaseDirectory;
		private static string dirName = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(current)).Name;
		private StandardFileSystem sfs = StandardFileSystem.Create(current);

		//standard file system
		[Test]
		public void SNullRoot()
		{
			Assert.Null(sfs.GetRoot().Parent);
		}

		[Test]
		public void SRootName()
		{
			StringAssert.AreEqualIgnoringCase("Debug", sfs.GetRoot().Name);
		}

		[Test]
		public void SDirName()
		{
			StdFSDir d = new StdFSDir(dirName);
			StringAssert.AreEqualIgnoringCase("Debug", d.Name);
		}

		[Test]
		public void SParseDir()
		{
			StdFSDir sfd = new StdFSDir(dirName);
			StringAssert.AreEqualIgnoringCase("Debug", sfd.Name);
		}

		[Test]
		public void SbadRoot()
		{
			StandardFileSystem sfs2 = StandardFileSystem.Create("asdf");

			Assert.Null(sfs2);
		}

		[Test]
		public void SdifferentRoot()
		{
			StandardFileSystem sfs2 = StandardFileSystem.Create("/Users");

			StringAssert.AreNotEqualIgnoringCase("/Users", sfs2.GetRoot().Name);
		}

		[Test]
		public void SgetDir_File()
		{
			StandardFileSystem sfs2 = StandardFileSystem.Create("/Users/drewm");
			Dir422 dir1 = sfs2.GetRoot().GetDir("Desktop");
			Assert.NotNull(dir1);


			Dir422 dir2 = dir1.GetDir("Ableton");
			Assert.NotNull(dir2);


			Dir422 dir3 = dir2.GetDir("wav files");
			Assert.NotNull(dir3);


			Assert.NotNull(dir3.GetFile("Cloud.wav"));
		}

		[Test]
		public void SSysContains()
		{
			StandardFileSystem sfs2 = StandardFileSystem.Create("/Users/drewm");

			Dir422 dir1 = sfs2.GetRoot().GetDir("Desktop");
			Dir422 dir2 = dir1.GetDir("Ableton");
			Dir422 dir3 = dir2.GetDir("wav files");

			File422 f = dir3.GetFile("Cloud.wav");

			Assert.IsTrue(sfs2.Contains(f));
		}

		[Test]
		public void SDirTests()
		{
			StandardFileSystem sfs2 = StandardFileSystem.Create("/Users/drewm");

			Dir422 dir1 = sfs2.GetRoot().GetDir("Desktop");

			//Assert.NotNull(dir1.CreateFile("hello.txt"));
			//Assert.NotNull(dir1.CreateFile("Test.txt"));

			Assert.NotNull(dir1.CreateFile("A"));
			string datastring = "new data";
			byte[] info = new UTF8Encoding(true).GetBytes(datastring);

			File422 a = dir1.GetFile("A");
			if(a is StdFSFile)
			{
				Assert.NotNull(a);
				a = a as StdFSFile;

				FileStream fs = (FileStream)((StdFSFile)a).OpenReadWrite();
				fs.Write(info, 0, info.Length);
				fs.Close();
			}

			else
			{
				Assert.IsTrue(false);
			}
		}

		[Test]
		public void SFileTests()
		{
			StandardFileSystem sfs2 = StandardFileSystem.Create("/Users/drewm");

			Dir422 dir1 = sfs2.GetRoot().GetDir("Desktop");
			Dir422 dir2 = dir1.GetDir("Ableton");
			Dir422 dir3 = dir2.GetDir("wav files");

			File422 f = dir3.GetFile("Cloud.wav");

			StringAssert.AreEqualIgnoringCase(f.Name, "Cloud.wav");
			StringAssert.AreEqualIgnoringCase(f.Parent.Name, "wav files");


		}

		//memory file system
		[Test]
		public void MNullRoot()
		{
			MemoryFileSystem mfs = new MemoryFileSystem();
			Assert.Null(mfs.GetRoot().Parent);
		}

		[Test]
		public void MRootName()
		{
			MemoryFileSystem mfs = new MemoryFileSystem();
			StringAssert.AreEqualIgnoringCase("root", mfs.GetRoot().Name);
		}

		[Test]
		public void MCreateDir()
		{
			MemoryFileSystem mfs = new MemoryFileSystem();
			Dir422 dir1 = mfs.GetRoot().CreateDir("Dir1");
			Dir422 dir2 = dir1.CreateDir("Dir2");

			Assert.NotNull(dir1);
			Assert.NotNull(mfs.Contains(dir1));

			Assert.False(mfs.GetRoot().ContainsDir("Dir2", false));
			Assert.True(mfs.GetRoot().ContainsDir("Dir2", true));
		}
	
		[Test]
		public void MCreateFile()
		{
			MemoryFileSystem mfs = new MemoryFileSystem();
			Dir422 dir1 = mfs.GetRoot().CreateDir("Dir1");
			File422 file1 = dir1.CreateFile("File1");

			Assert.NotNull(dir1);
			Assert.NotNull(mfs.Contains(dir1));

			Assert.False(mfs.GetRoot().ContainsFile("File1", false));
			Assert.True(mfs.GetRoot().ContainsFile("File1", true));
		}

		[Test]
		public void MReadWriteFile()
		{
			MemoryFileSystem mfs = new MemoryFileSystem();
			Dir422 dir1 = mfs.GetRoot().CreateDir("Dir1");
			File422 file1 = dir1.CreateFile("File1");

			string datastring = "new data";
			byte[] info = new UTF8Encoding(true).GetBytes(datastring);

			//Assert.NotNull(file1._stream);
			Stream open = file1.OpenReadWrite();
			Assert.True(true);
			Assert.NotNull(open);

			open.Write(info, 0, info.Length);

			open.Close();

			Stream read = file1.OpenReadWrite();
			byte[] readIn = new byte[256];
			read.Read(readIn, 0, 256);

			string str = System.Text.Encoding.Default.GetString(readIn).TrimEnd('\0');

			StringAssert.AreEqualIgnoringCase(str, datastring);
		}
	}
}
