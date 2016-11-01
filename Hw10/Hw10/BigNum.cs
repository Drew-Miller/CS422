using System;
using System.Text;

namespace CS422
{
	public class BigNum
	{
		string _number;

		public BigNum(string number)
		{
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

			//check whitespaces
			if(number.Contains(" "))
				return false;

			//check whitespaces
			if (number.Contains("\t"))
				return false;

			//check whitespaces
			if (number.Contains())
				return false;


			return true;
		}
	}
}
