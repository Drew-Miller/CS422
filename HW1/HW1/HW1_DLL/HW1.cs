using System;
using System.IO;
using System.Text;

namespace CS422
{
	//acts like the regular TextWriter object, but features the
	//ability to append a line number to the beginning of a string
	public class NumberedTextWriter : System.IO.TextWriter
	{
		//used to keep track of the line number for the class
		private int number;
		private System.IO.TextWriter TextWriter; 

		//constructor that only takes in a reference to a TextWriter
		//object and sets the line number to 1 by default
		public NumberedTextWriter(System.IO.TextWriter wrapThis)
		{
			TextWriter = wrapThis;
			number = 1;
		}

		//constructor that only takes in a reference to a TextWriter
		//object and sets the line number to an integer passed in.
		public NumberedTextWriter(System.IO.TextWriter wrapThis, int startingLineNumber)
		{
			TextWriter = wrapThis;
			number = startingLineNumber;
		}

		public override Encoding Encoding
		{
			get
			{
				return TextWriter.Encoding;
			}
		}

		//method to writeline like the regular TextWriter
		public override void WriteLine(string value)
		{
			//add the number line to the string
			value = number.ToString() + ": " + value;
			number++;

			//use the text writer that is wrapped to write the edited string
			TextWriter.WriteLine(value);
		}
	}

	//class that is a read only stream that keeps track
	//of the position of the stream
	public class IndexedNumsStream : System.IO.Stream
	{
		long length;
		long position;

		public IndexedNumsStream(long len)
		{
			if (len < 0)
			{
				length = 0;
			}

			else
			{
				length = len;
			}

			position = 0;
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return length;
			}
		}

		public override long Position
		{
			get
			{
				return position;
			}

			set
			{
				if(value < 0)
				{
					position = 0;
				}

				else if(value > length)
				{
					position = length;
				}

				else
				{
					position = value;
				}
			}
		}

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			//make sure offset is not negative
			if (offset < 0)
			{
				offset = 0;
			}

			if (count < 0)
			{
				count = 0;
			}

			//if the count is bigger than the length of the stream, we cannot
			//allocate all of those indices.
			if (count > length)
			{
				count = (int)length;
			}

			//if the offset is too high, we have to change that
			if (offset > length)
			{
				offset = (int)length;
			}

			//if the offset from the ammount requested is greater than the total
			//length, then we need to make the count smaller
			if (offset + count > length)
			{
				count = (int)(length - offset);
			}

			for (int i = 0; i < count; Position++, i++)
			{
				//if the values for offset does not fit within the buffer
				//we must break. else, continue until the count is satisfied or
				//we hit the end of the buffer.
				if((i + offset + 1) > buffer.Length)
				{
					break;
				}

				buffer[offset + i] = (byte)((Position) % 256);
			}

			return (int)position;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			if (value < 0)
			{
				length = 0;
			}

			else
			{
				length = value;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

	}
}

