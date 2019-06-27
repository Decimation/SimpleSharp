using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleSharp.Strings.Formatting
{
	public static class FormatUtils
	{
		private const char LB = '{';
		private const char RB = '}';

		public static string CreateFormatString(string formatSwitch)
		{
			const string FMT_ARG = "0:";
			const string DEF_FMT = "{0}";

			if (String.IsNullOrWhiteSpace(formatSwitch)) {
				formatSwitch = DEF_FMT;
				return formatSwitch;
			}

			if (!IsFormatSpecifier(formatSwitch)) {
				formatSwitch = LB + FMT_ARG + formatSwitch + RB;
			}

			return formatSwitch;
		}
		
		/// <summary>
		/// Checks whether <paramref name="format"/> is a valid format string.
		/// <example><c>{0:X}</c> returns <c>true</c></example>
		/// </summary>
		/// 
		/// <param name="format"><see cref="string"/> to check</param>
		/// <returns><c>true</c> if <paramref name="format"/> is a valid format string; <c>false</c> otherwise</returns>
		public static bool IsFormatSpecifier(string format)
		{
			const char COLON = ':';
			
			var chars = new Queue<char>(format);

			return chars.Dequeue() == LB 
			       && Char.IsNumber(chars.Dequeue()) 
			       && chars.Dequeue() == COLON
			       && Char.IsLetter(chars.Dequeue()) 
			       && chars.Last() == RB;
		}
	}
}