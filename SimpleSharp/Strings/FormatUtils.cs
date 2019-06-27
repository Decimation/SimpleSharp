using System.Collections.Generic;
using System.Linq;

namespace SimpleSharp.Strings
{
	public static class FormatUtils
	{
		public static bool IsFormatSpecifier(string format)
		{
			const char LB = '{';
			const char RB = '}';
			var chars = new Queue<char>(format);
//			return format.First() == LB && char.IsNumber(format[1]) && format[2] == ':' && char.IsLetter(format[3]) && format.Last() == RB;

			return chars.Dequeue() == LB 
			       && char.IsNumber(chars.Dequeue()) 
			       && chars.Dequeue() == ':' 
			       && char.IsLetter(chars.Dequeue()) 
			       && chars.Last() == RB;
		}
	}
}