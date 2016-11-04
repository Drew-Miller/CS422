using System;
using System.Text;

namespace CS422
{
	public class BigNum
	{
		string _number;

		public BigNum(string number)
		{
			if(!validateString(number))
			{
				throw new System.ArgumentException(number + ": Not a valid BigNum string");
			}

			_number = number;
		}

		//function to validate if the string is a valid number string
		public bool validateString(string number)
		{
			if (number == null)
				return false;
			//check the first character, can be '-','.', or a number
			if(number[0] != '-' && number[0] != '.' && !Char.IsNumber(number[0]))
				return false;

			if (number[0] == '.')
			{   
				if(number.Length <= 1)
					return false;
				if (number.Substring(1).Contains("."))
					return false;
			}

			if (number[0] == '-')
			{
				if (number.Length <= 1)
					return false;
				if (number.Substring(1).Contains("-"))
					return false;
			}
			
			//check whitespaces
			if(number.Contains(" "))
				return false;

			//check whitespaces
			if (number.Contains("\t"))
				return false;

			//check whitespaces
			if (number.Contains("\r"))
				return false;

			if (number.Contains("\v"))
				return false;


			return true;
		}
	}
}
