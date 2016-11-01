using System;

namespace CS422
{
	//constructors
	public class PCQueue
	{
		//members variables
		Node First;
		Node Last;

		public PCQueue()
		{
			First = null;
			Last = null;
		}

		public void print()
		{
			Node n = First;
			if (n == null)
			{
				Console.WriteLine("Empty");
			}

			while(n != null)
			{
				Console.WriteLine(n.Data);
				n = n.Next;
			}
		}

		public bool Dequeue(ref int out_value)
		{
			//if we cannot dequeue we return false
			if(First == null)
			{
				return false;
			}

			//we return the first item in the list
			else
			{
				out_value = First.Data;

				if (First.Next == null)
				{
					Last = null;
					First = null;

					return true;
				}

				else 
				{
					First = First.Next;
				}

				return true;
			}
		}

		//enqueues a new value to the linked list
		public bool Enqueue(int dataValue)
		{
			//if the list is empty, you set both
			//First and Last to null
			if (First == null)
			{
				First = new Node(dataValue);
				Last = First;

				return true;
			}

			//if the list is not empty, move the Last pointer
			//to the new value
			else if(First != null)
			{
				Last.Next = new Node(dataValue);
				Last = Last.Next;
				return true;
			}

			return false;
		}
	}

	//class for nodes that will be enqueued in the PCQueue class
	public class Node
	{
		//member variables
		private int data;
		Node next;

		//getters/setters
		public int Data
		{
			get 
			{ return data; }

			set 
			{ data = value; }
		}

		public Node Next
		{
			get 
			{ return next;}

			set 
			{ next = value;}
		}

		//contructors
		public Node()
		{
			next = null;
		}

		public Node(int d)
		{
			Data = d;
			next = null;
		}
	}
}

