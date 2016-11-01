using System;
using System.IO;

namespace CS422
{
	public class ConcatStream : Stream
	{
		Stream _f;
		Stream _s;
		long length;
		bool supportLength;
		long position;

		bool constructor;

		public ConcatStream(Stream first, Stream second)
		{
			constructor = false;
			supportLength = false;
			position = 0;
			_f = first;
			_s = second;

			//if we can seek, we do not implement forward only reading
			//if we cannot, we do implement forward only reading
			//test if we can seek on the first stream
			if (_f.CanSeek)
			{
				_f.Seek(0, SeekOrigin.Begin);
			}

			else { throw new NotSupportedException(); }

			if(_s.CanSeek)
			{
				_s.Seek(0, SeekOrigin.Begin);
			}

			//we have to have the first stream have a length
			try
			{
				long test = _f.Length;
			}

			//if the length of the first stream is not defined
			catch(NotSupportedException)
			{
				throw new ArgumentException();
			}

			//check if we can support the length property
			try
			{
				length = _f.Length + _s.Length;
				supportLength = true;
			}

			//if we can't support it, no big deal.
			//_s does not have to have a length property
			catch(NotSupportedException) {}
		}

		public ConcatStream(Stream first, Stream second, long fixedLength)
		{
			constructor = true;
			position = 0;
			_f = first;
			_s = second;

			supportLength = true;
			length = fixedLength;

			//if we can seek, we do not implement forward only reading
			//if we cannot, we do implement forward only reading
			if (_f.CanSeek)
			{
				_f.Seek(0, SeekOrigin.Begin);
			}

			else { throw new NotSupportedException();}

			if (_s.CanSeek)
			{
				_s.Seek(0, SeekOrigin.Begin);
			}

			//check if we can support the length property
			try
			{
				long test = _f.Length;
			}
			catch(NotSupportedException){}

			//check if we can support the length property
			try
			{
				//if we have an actual length that is supported by both streams
				if (length > _f.Length + _s.Length)
				{
					length = _f.Length + _s.Length;
				}
			
			}

			//if we can't support it, no big deal.
			//_s does not have to have a length property
			catch (NotSupportedException) { }
		}

		public override bool CanRead
		{
			get
			{
				if(_f != Null && _s != Null)
				{
					if (_f.CanRead == true && _s.CanRead == true)
					{
						return true;
					}
				}

				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				if (_f != Null && _s != Null)
				{
					if (_f.CanSeek == true && _s.CanSeek == true)
					{
						return true;
					}
				}

				return false;			
			}
		}

		public override bool CanWrite
		{
			get
			{
				if (_f != Null && _s != Null)
				{
					if (_f.CanWrite == true && _s.CanWrite == true)
					{
						return true;
					}
				}

				return false;
			}
		}

		public override long Length
		{
			get
			{
				//if we have a fixed length of the stream
				if(supportLength)
				{
					return length;
				}

				return -1;
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
				throw new NotImplementedException();
			}
		}

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		//implement
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!CanRead)
			{
				throw new NotImplementedException();
			}

			//while we have bytes to read
			int bytesRead = 0;
			for (int i = 0; i < count; i++)
			{
				//if we read from the first stream
				if(position < _f.Length)
				{
					bytesRead += _f.Read(buffer, offset + bytesRead, 1);
					position++;
				}

				//if we read from the second stream
				else
				{
					//read the rest of the stream
					int addition = 0;

					addition = _s.Read(buffer, offset + bytesRead, count - bytesRead);
					position += addition;
					bytesRead += addition;
											
					//if we have a fixed length and we used the second
					//constructor (or we were able to read a length)
					//we must resize.
					if(position >= length && (constructor || length > 0))
					{
						position = length - 1;
					}

					//break out because we have read all that is left
					break;
				}
			}

			return bytesRead;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if(CanSeek)
			{
				//if we seek to the start of the stream
				if(origin == SeekOrigin.Begin && offset >= 0)
				{
					//set the position to the start
					position = offset;
				}

				else if(origin == SeekOrigin.End && offset < 0)
				{
					position = _f.Length + _s.Length + offset;
				}

				else if(origin == SeekOrigin.Current)
				{
					position += offset;
				}

				//if not one of those seekOrigins, we throw exec
				else { throw new NotImplementedException();}

				//if the position is not a valid position
				if(position < 0 || position >= _f.Length + _s.Length)
				{
					throw new IndexOutOfRangeException();
				}

				//if we are still within the first stream
				if(position < _f.Length)
				{
					//seek to the beginning
					_f.Position = position;
				}

				//if we are in the second stream
				else
				{
					_s.Position = position - _f.Length;
				}

				return position;
			}

			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		//implement
		public override void Write(byte[] buffer, int offset, int count)
		{
			if (!CanWrite)
			{
				throw new NotImplementedException();
			}

			//if we can write
			for (int i = 0; i < count; i++)
			{
				//if we write to stream 1
				if(position < _f.Length)
				{
					_f.Write(buffer, offset + i, 1);
					position++;
				}

				//else, we write to stream 2
				else
				{
					//if the second stream cannot seek
					if(!_s.CanSeek)
					{
						try
						{
							//the exception is thrown if we are
							//not at the correct position to write in
							if(_s.Position != (position - _f.Length))
							{
								throw new Exception();
							}
						}

						//if we get an exception, throw it
						catch (Exception e) { throw e; }
					}

					//we can seek there!
					else
					{
						_s.Seek(position - _f.Length, SeekOrigin.Begin);
					}

					//write the rest to the buffer
					_s.Write(buffer, offset + i, count - i);
					break;
				}
			}

			//if stream cannot seek, check the position property
			//if it does not have one, throw an exception
		}
	}
}
