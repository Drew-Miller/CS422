using System;
using NUnit.Framework;
using System.IO;
using System.Text;

namespace CS422
{
	//class to test things
	[TestFixture()]
	public class NUnitTests
	{
		byte[] string1 = new UnicodeEncoding().GetBytes("My Test Stream");
		byte[] string2 = new UnicodeEncoding().GetBytes("Another Test Stream");

		[Test()]
		public void canSeekTest()
		{
			NoSeekMemoryStream nsms = new NoSeekMemoryStream(string1, 0, 2);
			Assert.IsFalse(nsms.CanSeek);
		}

		[Test()]
		public void seekBegin()
		{
			NoSeekMemoryStream nsms = new NoSeekMemoryStream(string1, 0, 2);
			Assert.Throws<NotSupportedException>(() => nsms.Seek(0, SeekOrigin.Begin));
		}

		[Test()]
		public void seekEnd()
		{
			NoSeekMemoryStream nsms = new NoSeekMemoryStream(string1, 0, 2);
			Assert.Throws<NotSupportedException>(() => nsms.Seek(2, SeekOrigin.End));
		}

		[Test()]
		public void testPos()
		{
			NoSeekMemoryStream nsms = new NoSeekMemoryStream(string1, 0, 2);
			Assert.Throws<NotSupportedException>(() => nsms.Position = 0);
		}

		[Test()]
		public void testConcatStreamEquals()
		{
			MemoryStream ms1 = new MemoryStream(string1);
			MemoryStream ms2 = new MemoryStream(string2);

			CS422.ConcatStream cs = new CS422.ConcatStream(ms1, ms2);

			byte[] appended = new byte[string1.Length + string2.Length];
			string1.CopyTo(appended, 0);
			string2.CopyTo(appended, string1.Length);

			bool equal = true;
			byte[] one = new byte[1];

			for (int i = 0; i < appended.Length; i++)
			{
				int read = cs.Read(one, 0, 1);
				if ((Convert.ToInt32(appended[i]) != Convert.ToInt32(one[0])) && read > 0)
				{
					equal = false;
				}
			}

			Assert.IsTrue(equal);
		}

		//1.
		[Test()]
		public void testConcatRead()
		{
			MemoryStream ms1 = new MemoryStream(string1);
			MemoryStream ms2 = new MemoryStream(string2);

			CS422.ConcatStream cs = new CS422.ConcatStream(ms1, ms2);

			byte[] buffer = new byte[ms1.Length + ms2.Length];

			Random rnd = new Random();
			int bytesRead = 0;

			//for every byte in the cs
			for (int i = 0; i < cs.Length; i++)
			{
				int r = rnd.Next(1, 4);

				if (r + bytesRead > cs.Length)
				{
					r = (int)cs.Length - bytesRead;
					bytesRead += cs.Read(buffer, bytesRead, r);
					break;
				}

				bytesRead += cs.Read(buffer, bytesRead, r);
			}

			byte[] appended = new byte[string1.Length + string2.Length];
			string1.CopyTo(appended, 0);
			string2.CopyTo(appended, string1.Length);

			CollectionAssert.AreEqual(appended, buffer);
		}

		//2.
		[Test()]
		public void testMemAnddNoSeek()
		{
			NoSeekMemoryStream ms1 = new NoSeekMemoryStream(string1);
			MemoryStream ms2 = new MemoryStream(string2);

			CS422.ConcatStream cs = new CS422.ConcatStream(ms2, ms1);

			byte[] buffer = new byte[ms1.Length + ms2.Length];

			Random rnd = new Random();
			int bytesRead = 0;

			//for every byte in the cs
			for (int i = 0; i < cs.Length; i++)
			{
				int r = rnd.Next(1, 10);

				if (r + bytesRead > cs.Length)
				{
					r = (int)cs.Length - bytesRead;
					bytesRead += cs.Read(buffer, bytesRead, r);
					break;
				}

				bytesRead += cs.Read(buffer, bytesRead, r);
			}

			byte[] appended = new byte[string1.Length + string2.Length];
			string2.CopyTo(appended, 0);
			string1.CopyTo(appended, string2.Length);

			CollectionAssert.AreEqual(appended, buffer);
		}

		//3.1
		[Test()]
		public void testNoSeekMemLength()
		{
			NoSeekMemoryStream ms1 = new NoSeekMemoryStream(string1);
			MemoryStream ms2 = new MemoryStream(string2);

			CS422.ConcatStream cs = new CS422.ConcatStream(ms2, ms1);

			byte[] appended = new byte[string1.Length + string2.Length];
			string1.CopyTo(appended, 0);
			string2.CopyTo(appended, string1.Length);

			Assert.AreEqual(appended.Length, cs.Length);
		}
	
		//3.2
		[Test()]
		public void concatStreamLength()
		{
			NoSeekMemoryStream ms1 = new NoSeekMemoryStream(string1);
			MemoryStream ms2 = new MemoryStream(string2);

			CS422.ConcatStream cs = new CS422.ConcatStream(ms2, ms1, 50);
			Assert.AreEqual(50, cs.Length);
		}

		//4.1
		[Test()]
		public void seekTest1()
		{
			NoSeekMemoryStream ms1 = new NoSeekMemoryStream(string1);
			Assert.Throws<NotSupportedException>(() => ms1.Seek(0, SeekOrigin.Begin));
		}

		//4.2
		[Test()]
		public void seekTest2()
		{
			NoSeekMemoryStream ms1 = new NoSeekMemoryStream(string1);
			Assert.Throws<NotSupportedException>(() => ms1.Position = 0);
		}
	}
}

