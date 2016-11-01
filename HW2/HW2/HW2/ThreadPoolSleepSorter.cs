using System;

namespace CS422
{
	public class ThreadPoolSleepSorter : IDisposable
	{
		public bool disposed;
		public System.IO.TextWriter TW;
		public System.Threading.Thread[] pool;

		public ThreadPoolSleepSorter(System.IO.TextWriter output, ushort threadcount)
		{
			disposed = false;
			TW = output;

			//create a pool with a set amount of threads;
			if (threadcount == 0)
			{
				pool = new System.Threading.Thread[64];
			}
			else
			{
				pool = new System.Threading.Thread[threadcount];
			}
		}

		public void Sort(byte[] values)
		{
			int i = 0;

			foreach(byte X in values)
			{
				pool[i] = new System.Threading.Thread(() => doWork(X));
				pool[i].Start();

				i++;
			}
		}



		//method for working
		public void doWork(byte val)
		{
			System.Threading.Thread.Sleep(val * 1000);
			TW.WriteLine(val);
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;
				foreach (System.Threading.Thread t in pool)
				{
					if(t != null)
					{
						t.Abort();
					}
				}
			}
		}

		public void Waste()
		{
			Dispose(true);
		}
	}
}

