using System;
using System.IO;
            
namespace CS422
{
	public class NoSeekMemoryStream : MemoryStream
	{
		public NoSeekMemoryStream(byte[] buffer)	
								: base(buffer)
		{
			
		}

		public NoSeekMemoryStream(byte[] buffer, int offset, int count)
								: base(buffer, offset, count)
		{
			
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Seek(long offset, SeekOrigin loc)
		{
			throw new NotSupportedException();
		}

		public override long Position
		{
			get
			{
				return base.Position;
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
